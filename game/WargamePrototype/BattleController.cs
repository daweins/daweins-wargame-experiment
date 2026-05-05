using Godot;
using Wargame.Core;

[GlobalClass]
public partial class BattleController : Control
{
    private const int BoardOriginX = 32;
    private const int BoardOriginY = 32;
    private const int BaseTileSize = 64;
    private const int LargeMapTileSize = 32;
    private const int BoardPixelHeight = 8 * BaseTileSize;
    private const int BoardPixelWidth = 12 * BaseTileSize;
    private const int HudChipGap = 8;
    private const int HudHeight = 188;
    private const int HudOriginX = BoardOriginX;
    private const int HudOriginY = BoardOriginY + BoardPixelHeight + 22;
    private const int HudWidth = BoardPixelWidth;
    private const int PanelContentWidth = 360;
    private const int PanelX = 840;
    private const float DamagePopupDuration = 0.95f;
    private const float HitFlashDuration = 0.34f;
    private const int SpriteSize = 64;
    private const int TileSize = BaseTileSize;

    private enum CampaignScreen
    {
        MissionIntro,
        MissionBattle,
        MissionOutro,
        CampaignComplete
    }

    private readonly Color _cover = new("#356c54");
    private readonly Color _enemy = new("#ff4f38");
    private readonly Color _enemyDark = new("#8f1824");
    private readonly Color _hq = new("#7457c7");
    private readonly Color _panel = new("#162130");
    private readonly Color _plain = new("#6c9e5f");
    private readonly Color _player = new("#38c7ff");
    private readonly Color _playerDark = new("#1558b0");
    private readonly Color _road = new("#b6945c");
    private readonly Color _text = new("#f0f2d4");
    private readonly Color _warning = new("#f6c85f");

    private readonly List<DamagePopup> _damagePopups = [];
    private readonly Dictionary<string, float> _displayHpByUnitId = new(StringComparer.Ordinal);
    private readonly Dictionary<string, float> _hitFlashByUnitId = new(StringComparer.Ordinal);
    private readonly List<string> _messages = [];
    private readonly Queue<BattleCommand> _autoplayCommands = [];
    private CampaignScreen _screen = CampaignScreen.MissionIntro;
    private int _missionNumber = 1;
    private BattleState _state = CampaignMissionFactory.Create(1);
    private BattleState? _pendingMoveUndoState;
    private TileCoord _cursor = new(2, 3);
    private TileCoord _pendingMoveOriginalCursor;
    private TileCoord _viewOrigin = new(0, 0);
    private string? _pendingMoveUnitId;
    private string? _selectedUnitId;
    private bool _actionMode;
    private bool _autoplayEnabled;
    private double _autoplayAccumulator;
    private int _autoplayActionsThisMission;
    private Texture2D? _commanderPortrait;
    private Texture2D? _mission1Concept;
    private Texture2D? _mission2Concept;
    private Texture2D? _pathSprites;
    private Texture2D? _terrainSprites;
    private Texture2D? _uiIconSprites;
    private Texture2D? _unitSprites;

    private int CurrentTileSize => _state.Width > 12 || _state.Height > 8 ? LargeMapTileSize : BaseTileSize;

    private int VisibleBoardColumns => BoardPixelWidth / CurrentTileSize;

    private int VisibleBoardRows => BoardPixelHeight / CurrentTileSize;

    private sealed class DamagePopup
    {
        public required int Amount { get; init; }

        public float Age { get; set; }

        public required TileCoord Coord { get; init; }

        public required bool IsCounterDamage { get; init; }

        public required bool IsDestroyed { get; init; }
    }

    private readonly record struct UnitSnapshot(string Id, Team Team, UnitType Type, TileCoord Position, int Hp, bool IsAlive);

    private readonly record struct ActiveForecast(UnitState Attacker, UnitState Defender, CombatForecast Forecast);

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(1280, 800);
        _terrainSprites = LoadSpriteSheet("res://assets/sprites/art_terrain.png")
            ?? LoadSpriteSheet("res://assets/sprites/terrain.png");
        _pathSprites = LoadSpriteSheet("res://assets/sprites/art_paths.png");
        _uiIconSprites = LoadSpriteSheet("res://assets/sprites/art_ui_icons.png")
            ?? LoadSpriteSheet("res://assets/sprites/ui_icons.png");
        _unitSprites = LoadSpriteSheet("res://assets/sprites/art_units.png")
            ?? LoadSpriteSheet("res://assets/sprites/campaign_units.png")
            ?? LoadSpriteSheet("res://assets/sprites/units.png");
        _mission1Concept = LoadSpriteSheet("res://assets/art-handoff/requests/03-mission-one-cutscene-frame/ChatGPT Image May 2, 2026, 09_41_00 PM.png");
        _mission2Concept = LoadSpriteSheet("res://assets/art-handoff/requests/04-mission-two-relay-yard-concept/ChatGPT Image May 2, 2026, 09_42_20 PM.png");
        _commanderPortrait = LoadSpriteSheet("res://assets/art-handoff/requests/10-missions-01-10-imagery-thread/local-venn-portrait-v3.png")
            ?? LoadSpriteSheet("res://assets/art-handoff/requests/05-character-portrait-concept/ChatGPT Image May 2, 2026, 09_44_20 PM.png");
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (IsCutsceneScreen())
        {
            DrawCutsceneScreen();
            return;
        }

        DrawBackdrop();
        DrawBoard();
        DrawHighlights();
        DrawUnits();
        DrawMissionMarkers();
        DrawCombatFeedback();
        DrawCursor();
        DrawArenaHud();
        DrawPanel();
    }

    public override void _Process(double delta)
    {
        if (_autoplayEnabled)
        {
            AdvanceAutoplay(delta);
        }

        if (UpdateCombatFeedback((float)delta))
        {
            QueueRedraw();
        }
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (inputEvent.IsEcho())
        {
            return;
        }

        if (IsCutsceneScreen())
        {
            if (inputEvent.IsActionPressed("ui_accept") || inputEvent.IsActionPressed("ui_cancel"))
            {
                AdvanceCutscene();
            }

            if (inputEvent is InputEventKey { Pressed: true } cutsceneKeyEvent && cutsceneKeyEvent.Keycode == Key.R)
            {
                AdvanceCutscene();
            }

            return;
        }

        if (inputEvent.IsActionPressed("ui_right")) MoveCursor(new TileCoord(_cursor.X + 1, _cursor.Y));
        if (inputEvent.IsActionPressed("ui_left")) MoveCursor(new TileCoord(_cursor.X - 1, _cursor.Y));
        if (inputEvent.IsActionPressed("ui_down")) MoveCursor(new TileCoord(_cursor.X, _cursor.Y + 1));
        if (inputEvent.IsActionPressed("ui_up")) MoveCursor(new TileCoord(_cursor.X, _cursor.Y - 1));
        if (inputEvent.IsActionPressed("ui_accept")) Confirm();
        if (inputEvent.IsActionPressed("ui_cancel")) CancelSelection();

        if (inputEvent is InputEventKey { Pressed: true } keyEvent)
        {
            if (keyEvent.Keycode == Key.E) EndTurn();
            if (keyEvent.Keycode == Key.R) RetryOrAdvanceCompleteMission();
            if (keyEvent.Keycode == Key.Tab) CycleReadyUnit();
            if (keyEvent.Keycode == Key.F9) ToggleAutoplay();
        }

        if (inputEvent is InputEventJoypadButton { Pressed: true } joypadButton)
        {
            if (joypadButton.ButtonIndex == JoyButton.Start) EndTurn();
            if (joypadButton.ButtonIndex == JoyButton.RightShoulder) CycleReadyUnit();
        }
    }

    private void AddMessage(string message)
    {
        _messages.Insert(0, message);
        if (_messages.Count > 5)
        {
            _messages.RemoveAt(_messages.Count - 1);
        }
    }

    private void AdvanceAutoplay(double delta)
    {
        _autoplayAccumulator += delta;
        if (_autoplayAccumulator < 0.18)
        {
            return;
        }

        _autoplayAccumulator = 0;

        if (IsCutsceneScreen())
        {
            AdvanceCutscene();
            return;
        }

        if (_state.IsComplete)
        {
            if (_state.Outcome == BattleOutcome.PlayerVictory)
            {
                ShowMissionOutro();
            }
            else
            {
                AddMessage("Autoplay stopped on defeat. Press F9 to resume after retry.");
                _autoplayEnabled = false;
            }

            return;
        }

        _selectedUnitId = null;
        _actionMode = false;
        _pendingMoveUndoState = null;
        _pendingMoveUnitId = null;

        if (_autoplayCommands.Count == 0)
        {
            foreach (var command in CampaignAutoplayer.ChoosePlayerTurn(_state))
            {
                _autoplayCommands.Enqueue(command);
            }
        }

        if (_autoplayCommands.TryDequeue(out var nextCommand))
        {
            AddMessage($"AUTO {CampaignAutoplayer.DescribeCommand(nextCommand)}");
            ApplyAndReport(nextCommand);
            _autoplayActionsThisMission++;
            return;
        }

        var beforeEnemyPhase = CaptureUnitSnapshots();
        ApplyAndReport(BattleCommand.EndTurn());
        ReportEnemyPhase(beforeEnemyPhase);
        _autoplayActionsThisMission++;
    }

    private void ToggleAutoplay()
    {
        _autoplayEnabled = !_autoplayEnabled;
        _autoplayAccumulator = 0;
        _autoplayCommands.Clear();
        AddMessage(_autoplayEnabled
            ? "AI playtest enabled. Blue and red are now automated."
            : "AI playtest paused. Manual control restored.");
        QueueRedraw();
    }

    private void CancelSelection()
    {
        if (_selectedUnitId is null)
        {
            return;
        }

        if (_actionMode && _pendingMoveUndoState is not null && _pendingMoveUnitId == _selectedUnitId)
        {
            _state = _pendingMoveUndoState;
            _cursor = _pendingMoveOriginalCursor;
            _actionMode = false;
            _pendingMoveUndoState = null;
            _pendingMoveUnitId = null;
            AddMessage("Move undone. Pick a different blue tile, or Esc/B again to cancel selection.");
            QueueRedraw();
            return;
        }

        _selectedUnitId = null;
        _actionMode = false;
        _pendingMoveUndoState = null;
        _pendingMoveUnitId = null;
        AddMessage("Selection cancelled.");
        QueueRedraw();
    }

    private void Confirm()
    {
        if (_state.IsComplete)
        {
            if (_state.Outcome == BattleOutcome.PlayerVictory)
            {
                ShowMissionOutro();
                return;
            }

            ResetMission();
            return;
        }

        if (_selectedUnitId is null)
        {
            TrySelectUnit();
            return;
        }

        var selectedUnit = _state.Units.FirstOrDefault(unit => unit.Id == _selectedUnitId && unit.IsAlive);
        if (selectedUnit is null)
        {
            CancelSelection();
            return;
        }

        var targetUnit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (targetUnit is { Team: Team.Enemy } && selectedUnit.Position.DistanceTo(targetUnit.Position) == 1)
        {
            ApplyAndReport(BattleCommand.Attack(selectedUnit.Id, targetUnit.Id));
            _selectedUnitId = null;
            _actionMode = false;
            _pendingMoveUndoState = null;
            _pendingMoveUnitId = null;
            return;
        }

        if (!_actionMode)
        {
            var reachable = BattleRules.GetReachableTiles(_state, selectedUnit);
            if (reachable.Contains(_cursor))
            {
                var undoState = _state.Clone();
                var originalCursor = selectedUnit.Position;
                var result = ApplyAndReport(BattleCommand.Move(selectedUnit.Id, _cursor));
                if (result.Success)
                {
                    _pendingMoveUndoState = undoState;
                    _pendingMoveOriginalCursor = originalCursor;
                    _pendingMoveUnitId = selectedUnit.Id;
                    _actionMode = true;
                    AddMessage("Action mode: attack, wait, or Esc/B to undo that move.");
                }

                QueueRedraw();
                return;
            }

            AddMessage("That tile is not reachable.");
            QueueRedraw();
            return;
        }

        if (_cursor == selectedUnit.Position)
        {
            ApplyAndReport(BattleCommand.Wait(selectedUnit.Id));
            _selectedUnitId = null;
            _actionMode = false;
            _pendingMoveUndoState = null;
            _pendingMoveUnitId = null;
            return;
        }

        AddMessage("Action needs an adjacent red unit, or Enter/A on your unit to wait. Esc/B undoes the move.");
        QueueRedraw();
    }

    private CommandResult ApplyAndReport(BattleCommand command)
    {
        var beforeCommand = command.Kind == CommandKind.Attack
            ? CaptureUnitSnapshots()
            : null;
        var wasScoutRescued = _state.ScoutRescued;
        var result = BattleRules.ApplyCommand(_state, command);
        if (result.Success && beforeCommand is not null)
        {
            ApplyCombatFeedback(command, beforeCommand);
        }

        AddMessage(result.Message);
        if (!wasScoutRescued && _state.ScoutRescued)
        {
            AddMessage("Scout-7 secured. They can act on later turns if they survive.");
        }

        if (_state.Outcome == BattleOutcome.PlayerVictory)
        {
            AddMessage($"Mission complete. {_state.VictoryLine}");
        }
        else if (_state.Outcome == BattleOutcome.PlayerDefeat)
        {
            AddMessage(DefeatReasonText());
            AddMessage("Mission failed. Press R or Enter/A to retry.");
        }

        QueueRedraw();
        return result;
    }

    private void CycleReadyUnit()
    {
        var readyUnits = _state.Units
            .Where(unit => unit.Team == Team.Player && unit.IsAlive && !unit.HasActed && !BattleRules.IsScoutStranded(_state, unit))
            .OrderBy(unit => unit.Id, StringComparer.Ordinal)
            .ToList();

        if (readyUnits.Count == 0)
        {
            AddMessage("No ready units. End turn to let the raiders make regrettable choices.");
            QueueRedraw();
            return;
        }

        var selectedIndex = readyUnits.FindIndex(unit => unit.Id == _selectedUnitId);
        var nextUnit = readyUnits[(selectedIndex + 1 + readyUnits.Count) % readyUnits.Count];
        _cursor = nextUnit.Position;
        UpdateViewOrigin();
        _selectedUnitId = nextUnit.Id;
        _actionMode = nextUnit.HasMoved;
        _pendingMoveUndoState = null;
        _pendingMoveUnitId = null;
        QueueRedraw();
    }

    private void DrawBoard()
    {
        var boardRect = BoardRect();
        DrawRect(boardRect.Grow(14), new Color("#070b12"));
        DrawRect(boardRect.Grow(10), new Color("#26364a"));
        DrawRect(boardRect.Grow(6), new Color("#0d1420"));

        for (var row = _viewOrigin.Y; row < Math.Min(_state.Height, _viewOrigin.Y + VisibleBoardRows); row++)
        {
            for (var column = _viewOrigin.X; column < Math.Min(_state.Width, _viewOrigin.X + VisibleBoardColumns); column++)
            {
                var coord = new TileCoord(column, row);
                var rect = TileRect(coord);
                DrawRect(rect, TileColor(_state.GetTerrain(coord)));
                DrawTerrainSprite(_state.GetTerrain(coord), rect, coord);
            }
        }

        DrawRect(boardRect.Grow(2), new Color("#f2d17a"), filled: false, width: 3);
        DrawViewportBadge();
    }

    private void DrawCursor()
    {
        var rect = TileRect(_cursor);
        DrawRect(rect.Grow(-3), new Color("#050910"), filled: false, width: 6);
        DrawRect(rect.Grow(-5), _warning, filled: false, width: 3);
        DrawRect(new Rect2(rect.Position + new Vector2(7, 7), new Vector2(14, 6)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(7, 7), new Vector2(6, 14)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - 21, 7), new Vector2(14, 6)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - 13, 7), new Vector2(6, 14)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(7, rect.Size.Y - 13), new Vector2(14, 6)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(7, rect.Size.Y - 21), new Vector2(6, 14)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - 21, rect.Size.Y - 13), new Vector2(14, 6)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - 13, rect.Size.Y - 21), new Vector2(6, 14)), _warning);
    }

    private void DrawHighlights()
    {
        if (_selectedUnitId is null)
        {
            return;
        }

        var selectedUnit = _state.Units.FirstOrDefault(unit => unit.Id == _selectedUnitId && unit.IsAlive);
        if (selectedUnit is null)
        {
            return;
        }

        if (!_actionMode)
        {
            foreach (var coord in BattleRules.GetReachableTiles(_state, selectedUnit))
            {
                if (!IsTileVisible(coord))
                {
                    continue;
                }

                var rect = TileRect(coord).Grow(-11);
                DrawRect(rect, new Color("#62d7ff40"));
                DrawRect(rect, new Color("#9ff0ff9a"), filled: false, width: 2);
            }
        }

        foreach (var coord in BattleRules.GetAttackableCoords(_state, selectedUnit))
        {
            if (!IsTileVisible(coord))
            {
                continue;
            }

            var rect = TileRect(coord).Grow(-9);
            DrawRect(rect, new Color("#ff655551"));
            DrawRect(rect, new Color("#ffd1bd"), filled: false, width: 2);
        }

        DrawTargetingChip(selectedUnit);
    }

    private void DrawArenaHud()
    {
        var font = GetThemeDefaultFont();
        var hudRect = new Rect2(HudOriginX, HudOriginY, HudWidth, HudHeight);
        DrawRect(hudRect, new Color("#0d1420e8"));
        DrawRect(new Rect2(hudRect.Position, new Vector2(hudRect.Size.X, 36)), new Color("#142235"));
        DrawRect(hudRect, new Color("#31455e"), filled: false, width: 2);

        var enemiesAlive = _state.Units.Count(unit => unit.Team == Team.Enemy && unit.IsAlive);
        var topY = HudOriginY + 10;
        var topX = HudOriginX + 12;
        DrawHudChip(font, new Rect2(topX, topY, 88, 30), $"TURN {_state.Turn}", _warning, 16);
        topX += 88 + HudChipGap;
        DrawHudChip(font, new Rect2(topX, topY, 264, 30), ObjectiveHudText(), _warning, 16, 9);
        topX += 264 + HudChipGap;
        DrawHudChip(font, new Rect2(topX, topY, 118, 30), HqHudText(), IsHqThreatened() ? _enemy : _player, 16, 10);
        topX += 118 + HudChipGap;
        DrawHudChip(font, new Rect2(topX, topY, 160, 30), ScoutHudText(), _state.ScoutRescued ? _player : _warning, 16, _state.RelayStation != TileCoord.None ? 3 : 11);
        topX += 160 + HudChipGap;
        DrawHudChip(font, new Rect2(topX, topY, 86, 30), $"RED {enemiesAlive}", enemiesAlive == 0 ? _player : _enemy, 16);

        var inspectY = HudOriginY + 52;
        DrawHudChip(font, new Rect2(HudOriginX + 12, inspectY, 240, 32), CursorHudText(), _warning, 17);
        DrawHudChip(font, new Rect2(HudOriginX + 260, inspectY, 170, 32), CursorTerrainHudText(), TerrainHudColor(_state.GetTerrain(_cursor)), 17);
        DrawHudChip(font, new Rect2(HudOriginX + 438, inspectY, 154, 32), SelectedHudText(), _selectedUnitId is null ? _warning : _player, 17);
        DrawHudChip(font, new Rect2(HudOriginX + 600, inspectY, 156, 32), ModeTitle(), ModeColor(), 17);

        DrawArenaForecastBand(font, HudOriginY + 96);
        DrawPromptRail(font, HudOriginY + 142);
    }

    private void DrawArenaForecastBand(Font font, int yPosition)
    {
        var activeForecast = GetActiveForecast();
        if (activeForecast is null)
        {
            DrawHudChip(font, new Rect2(HudOriginX + 12, yPosition, 356, 34), ArenaHintText(), _warning, 18);
            DrawHudChip(font, new Rect2(HudOriginX + 376, yPosition, 380, 34), TerrainValueText(_state.GetTerrain(_cursor)), TerrainHudColor(_state.GetTerrain(_cursor)), 18);
            return;
        }

        var forecast = activeForecast.Value.Forecast;
        var attacker = activeForecast.Value.Attacker;
        var defender = activeForecast.Value.Defender;
        DrawHudChip(font, new Rect2(HudOriginX + 12, yPosition, 228, 34), $"ATTACK -{forecast.MinimumDamage}-{forecast.MaximumDamage} HP", _enemy, 18);
        DrawHudChip(font, new Rect2(HudOriginX + 248, yPosition, 218, 34), CounterHudText(forecast), forecast.CounterMaximumDamage > 0 ? _warning : _player, 18);
        DrawHudChip(font, new Rect2(HudOriginX + 474, yPosition, 282, 34), ForecastTerrainHudText(attacker, defender), TerrainHudColor(_state.GetTerrain(defender.Position)), 18);
    }

    private void DrawHudChip(Font font, Rect2 rect, string text, Color accent, int fontSize, int iconIndex = -1)
    {
        DrawRect(new Rect2(rect.Position + new Vector2(3, 3), rect.Size), new Color("#0509108f"));
        DrawRect(rect, new Color("#101820"));
        DrawRect(new Rect2(rect.Position, new Vector2(6, rect.Size.Y)), accent);
        DrawRect(rect, accent, filled: false, width: 1);
        var textX = 13;
        if (iconIndex >= 0)
        {
            var iconRect = new Rect2(rect.Position + new Vector2(12, 5), new Vector2(20, 20));
            DrawUiIcon(iconRect, iconIndex);
            textX = 38;
        }

        DrawString(font, rect.Position + new Vector2(textX, rect.Size.Y - 9), text, HorizontalAlignment.Left, rect.Size.X - textX - 6, fontSize, _text);
    }

    private void DrawMissionMarkers()
    {
        var font = GetThemeDefaultFont();
        DrawObjectiveBeacon(font, _state.PlayerHq, "HQ", IsHqThreatened() ? _enemy : _warning);
        if (_state.RelayStation != TileCoord.None)
        {
            DrawObjectiveBeacon(font, _state.RelayStation, ObjectiveMarkerText("RLY", _state.RelayCaptureProgress, _state.RelaySecured), _state.RelaySecured ? _player : _warning);
        }

        if (_state.FuelCache != TileCoord.None)
        {
            DrawObjectiveBeacon(font, _state.FuelCache, ObjectiveMarkerText("FUEL", _state.FuelCaptureProgress, _state.FuelSecured), _state.FuelSecured ? _player : _warning);
        }

        DrawScoutRescueMarkers(font);
        DrawTerrainDefensePips(_cursor, TerrainDefenseValue(_state.GetTerrain(_cursor)), _warning);

        var activeForecast = GetActiveForecast();
        if (activeForecast is not null)
        {
            var defenderTerrain = _state.GetTerrain(activeForecast.Value.Defender.Position);
            DrawTerrainDefensePips(activeForecast.Value.Defender.Position, TerrainDefenseValue(defenderTerrain), _enemy);
        }
    }

    private void DrawObjectiveBeacon(Font font, TileCoord coord, string label, Color color)
    {
        if (!IsTileVisible(coord))
        {
            return;
        }

        var rect = TileRect(coord);
        var width = Math.Clamp(18 + label.Length * 7, 32, 54);
        var beacon = new Rect2(rect.Position + new Vector2((rect.Size.X - width) / 2f, 8), new Vector2(width, 22));
        DrawRect(new Rect2(beacon.Position + new Vector2(3, 3), beacon.Size), new Color("#050910b8"));
        DrawRect(beacon, new Color("#101820e8"));
        DrawRect(beacon, color, filled: false, width: 2);
        DrawString(font, beacon.Position + new Vector2(5, 16), label, HorizontalAlignment.Center, beacon.Size.X - 10, 13, color);
    }

    private void DrawPromptRail(Font font, int yPosition)
    {
        var xPosition = HudOriginX + 12;
        DrawHudChip(font, new Rect2(xPosition, yPosition, 118, 34), PrimaryPromptText(), ModeColor(), 18, PrimaryPromptIconIndex());
        xPosition += 118 + HudChipGap;
        DrawHudChip(font, new Rect2(xPosition, yPosition, 118, 34), CancelPromptText(), _warning, 18);
        xPosition += 118 + HudChipGap;
        DrawHudChip(font, new Rect2(xPosition, yPosition, 118, 34), "RB NEXT", _player, 18, 0);
        xPosition += 118 + HudChipGap;
        DrawHudChip(font, new Rect2(xPosition, yPosition, 128, 34), "START END", _enemy, 18, 7);
        xPosition += 128 + HudChipGap;
        DrawHudChip(font, new Rect2(xPosition, yPosition, 110, 34), "R RESET", _warning, 18, 2);
    }

    private void DrawScoutRescueMarkers(Font font)
    {
        if (!_state.RequiresScoutSurvival)
        {
            return;
        }

        var scout = _state.Units.FirstOrDefault(unit => unit.Id == _state.ScoutId && unit.IsAlive);
        if (scout is null)
        {
            return;
        }

        if (_state.ScoutRescued)
        {
            DrawObjectiveBeacon(font, scout.Position, "OK", _player);
            return;
        }

        var reachableRescueTiles = new HashSet<TileCoord>();
        var selectedUnit = SelectedUnit();
        if (selectedUnit is not null && CanRescueScout(selectedUnit))
        {
            reachableRescueTiles = BattleRules.GetReachableTiles(_state, selectedUnit).ToHashSet();
        }

        foreach (var coord in scout.Position.Neighbors().Where(_state.Contains))
        {
            if (_state.GetTerrain(coord) is TerrainType.Ridge or TerrainType.River)
            {
                continue;
            }

            if (BattleRules.GetLivingUnitAt(_state, coord) is not null)
            {
                continue;
            }

            var isReachable = reachableRescueTiles.Contains(coord);
            if (!IsTileVisible(coord))
            {
                continue;
            }

            var markerColor = isReachable ? _player : _warning;
            var rect = TileRect(coord).Grow(-7);
            DrawRect(rect, new Color(isReachable ? "#4aa4e833" : "#f6c85f25"));
            DrawTileCorners(rect, markerColor, 13, 3);
        }
    }

    private void DrawTerrainDefensePips(TileCoord coord, int defense, Color color)
    {
        if (defense <= 0 || !IsTileVisible(coord))
        {
            return;
        }

        var rect = TileRect(coord);
        var start = rect.Position + new Vector2(7, rect.Size.Y - 21);
        for (var index = 0; index < defense; index++)
        {
            var pip = new Rect2(start + new Vector2(index * 9, 0), new Vector2(7, 11));
            DrawRect(new Rect2(pip.Position + new Vector2(1, 1), pip.Size), new Color("#050910b8"));
            DrawRect(pip, color);
            DrawRect(pip.Grow(-2), new Color("#fff4b060"));
        }
    }

    private void DrawTileCorners(Rect2 rect, Color color, int length, int thickness)
    {
        DrawRect(new Rect2(rect.Position, new Vector2(length, thickness)), color);
        DrawRect(new Rect2(rect.Position, new Vector2(thickness, length)), color);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - length, 0), new Vector2(length, thickness)), color);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - thickness, 0), new Vector2(thickness, length)), color);
        DrawRect(new Rect2(rect.Position + new Vector2(0, rect.Size.Y - thickness), new Vector2(length, thickness)), color);
        DrawRect(new Rect2(rect.Position + new Vector2(0, rect.Size.Y - length), new Vector2(thickness, length)), color);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - length, rect.Size.Y - thickness), new Vector2(length, thickness)), color);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - thickness, rect.Size.Y - length), new Vector2(thickness, length)), color);
    }

    private void DrawDashedRect(Rect2 rect, Color color, int dashLength, int thickness)
    {
        for (var offset = 0; offset < rect.Size.X; offset += dashLength * 2)
        {
            var length = Math.Min(dashLength, (int)rect.Size.X - offset);
            DrawRect(new Rect2(rect.Position + new Vector2(offset, 0), new Vector2(length, thickness)), color);
            DrawRect(new Rect2(rect.Position + new Vector2(offset, rect.Size.Y - thickness), new Vector2(length, thickness)), color);
        }

        for (var offset = 0; offset < rect.Size.Y; offset += dashLength * 2)
        {
            var length = Math.Min(dashLength, (int)rect.Size.Y - offset);
            DrawRect(new Rect2(rect.Position + new Vector2(0, offset), new Vector2(thickness, length)), color);
            DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - thickness, offset), new Vector2(thickness, length)), color);
        }
    }

    private void DrawBackdrop()
    {
        DrawRect(new Rect2(Vector2.Zero, Size), new Color("#0b111b"));
        DrawRect(new Rect2(0, 0, Size.X, 94), new Color("#111c2a"));
        DrawRect(new Rect2(0, 94, Size.X, 4), new Color("#2c435c"));
        DrawRect(new Rect2(0, 760, Size.X, 40), new Color("#070b12"));

        for (var x = 18; x < Size.X; x += 54)
        {
            var y = 12 + ((x / 54) % 3) * 18;
            DrawRect(new Rect2(x, y, 18, 4), new Color("#26364a"));
            DrawRect(new Rect2(x + 8, y + 10, 26, 4), new Color("#1c2a3d"));
        }
    }

    private void DrawPanel()
    {
        var panelRect = new Rect2(PanelX, 32, 408, 720);
        DrawRect(new Rect2(panelRect.Position + new Vector2(8, 8), panelRect.Size), new Color("#050910aa"));
        DrawRect(panelRect, _panel);
        DrawRect(panelRect, new Color("#354a63"), filled: false, width: 3);
        DrawRect(new Rect2(PanelX, 32, 408, 54), new Color("#21334a"));
        DrawRect(new Rect2(PanelX, 82, 408, 4), _warning);
        var font = GetThemeDefaultFont();
        var yPosition = 58;
        DrawString(font, new Vector2(PanelX + 24, yPosition), $"OPERATION: {_state.MissionTitle.ToUpperInvariant()}", HorizontalAlignment.Left, 356, 18, _text);
        yPosition += 28;

        DrawModeBanner(font, ref yPosition);
        yPosition += 10;

        DrawSectionHeader(font, "OBJECTIVE", ref yPosition);
        yPosition = DrawWrappedLine(font, $"Turn {_state.Turn}: {ObjectiveText()}", yPosition, 16, _text, 36, 2);
        yPosition = DrawWrappedLine(font, RescueInstructionText(), yPosition + 4, 15, _warning, 34, 3);
        yPosition = DrawWrappedLine(font, ModeInstructionText(), yPosition + 8, 15, _text, 36, 3);
        yPosition = DrawWrappedLine(font, "E/Start: end turn. Tab/RB: cycle ready units. F9: AI playtest. Esc/B: cancel.", yPosition + 4, 14, _text, 38, 3);
        yPosition += 8;

        DrawSectionHeader(font, "FIELD GUIDE", ref yPosition);
        DrawLegend(font, ref yPosition);
        yPosition += 10;

        DrawSectionHeader(font, "INSPECT", ref yPosition);
        var cursorUnit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (cursorUnit is not null)
        {
            var profile = BattleRules.GetProfile(cursorUnit.Type);
            yPosition = DrawWrappedLine(font, $"Cursor: {TeamName(cursorUnit.Team)} {cursorUnit.Type} HP {cursorUnit.Hp}/{profile.MaxHp}", yPosition, 16, _text, 36, 2);
            yPosition = DrawWrappedLine(font, UnitStatsText(cursorUnit), yPosition, 14, _warning, 39, 2);
            yPosition = DrawWrappedLine(font, "ATK raises damage. DEF and cover reduce incoming damage.", yPosition, 13, _text, 42, 2);
            yPosition = DrawWrappedLine(font, UnitRoleText(cursorUnit.Type), yPosition, 14, _text, 39, 2);
        }
        else
        {
            yPosition = DrawWrappedLine(font, $"Cursor: {_state.GetTerrain(_cursor)} {_cursor}", yPosition, 16, _text, 36, 1);
        }

        DrawForecast(font, ref yPosition);

        yPosition += 10;
        DrawSectionHeader(font, "LOG", ref yPosition);
        foreach (var message in _messages)
        {
            yPosition = DrawWrappedLine(font, message, yPosition, 13, _text, 42, 2);
            yPosition += 2;
        }

        if (_state.IsComplete)
        {
            DrawScore(font);
        }
    }

    private void DrawForecast(Font font, ref int yPosition)
    {
        if (_selectedUnitId is null)
        {
            return;
        }

        var selectedUnit = _state.Units.FirstOrDefault(unit => unit.Id == _selectedUnitId && unit.IsAlive);
        var targetUnit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (selectedUnit is null || targetUnit is null || targetUnit.Team == selectedUnit.Team || selectedUnit.Position.DistanceTo(targetUnit.Position) != 1)
        {
            return;
        }

        var forecast = BattleRules.GetCombatForecast(_state, selectedUnit, targetUnit);
        yPosition = DrawWrappedLine(font, $"Attack forecast: target loses {forecast.MinimumDamage}-{forecast.MaximumDamage} HP.", yPosition + 4, 16, _warning, 39, 2);
        yPosition = DrawWrappedLine(font, $"Counter: you lose {forecast.CounterMinimumDamage}-{forecast.CounterMaximumDamage} HP if they can fire back.", yPosition, 15, _warning, 41, 2);
        yPosition = DrawWrappedLine(font, ForecastMathText(selectedUnit, targetUnit), yPosition, 13, _text, 43, 2);
    }

    private void DrawFallbackTerrainPattern(TerrainType terrain, Rect2 rect, TileCoord coord)
    {
        switch (terrain)
        {
            case TerrainType.Plain:
                DrawRect(TileLocalRect(rect, 10, 12, 12, 4), new Color("#83b96c"));
                DrawRect(TileLocalRect(rect, 34, 22, 18, 5), new Color("#527f4f"));
                DrawRect(TileLocalRect(rect, 19, 42, 11, 4), new Color("#7db467"));
                break;
            case TerrainType.Road:
                DrawRoadFallback(rect, coord);
                break;
            case TerrainType.Cover:
                DrawRect(TileLocalRect(rect, 8, 29, 48, 18), new Color("#244638"));
                DrawRect(TileLocalRect(rect, 11, 15, 16, 17), new Color("#5aa06b"));
                DrawRect(TileLocalRect(rect, 18, 8, 12, 13), new Color("#7fc279"));
                DrawRect(TileLocalRect(rect, 34, 18, 16, 18), new Color("#437c5b"));
                DrawRect(TileLocalRect(rect, 39, 10, 12, 12), new Color("#6aae70"));
                DrawRect(TileLocalRect(rect, 21, 32, 5, 17), new Color("#4a3227"));
                DrawRect(TileLocalRect(rect, 42, 34, 5, 16), new Color("#4a3227"));
                break;
            case TerrainType.Hq:
                DrawRect(TileLocalRect(rect, 10, 42, 44, 10), new Color("#3c2e67"));
                DrawRect(TileLocalRect(rect, 16, 20, 32, 28), new Color("#c9b9ff"));
                DrawRect(TileLocalRect(rect, 20, 16, 24, 8), _warning);
                DrawRect(TileLocalRect(rect, 24, 30, 7, 10), new Color("#5e45a6"));
                DrawRect(TileLocalRect(rect, 36, 30, 7, 10), new Color("#5e45a6"));
                DrawRect(TileLocalRect(rect, 18, 48, 30, 4), new Color("#f0e7ff"));
                break;
            case TerrainType.Ridge:
                DrawRect(TileLocalRect(rect, 7, 34, 50, 14), new Color("#111926"));
                DrawRect(TileLocalRect(rect, 11, 26, 18, 15), new Color("#38455a"));
                DrawRect(TileLocalRect(rect, 25, 18, 19, 23), new Color("#4c5a70"));
                DrawRect(TileLocalRect(rect, 40, 29, 14, 12), new Color("#2b3547"));
                DrawRect(TileLocalRect(rect, 26, 19, 10, 4), new Color("#8a96aa"));
                break;
            case TerrainType.River:
                DrawRiverFallback(rect, coord);
                break;
            case TerrainType.Workshop:
                DrawRect(TileLocalRect(rect, 8, 38, 48, 12), new Color("#2a2836"));
                DrawRect(TileLocalRect(rect, 13, 19, 38, 27), new Color("#7a6a3f"));
                DrawRect(TileLocalRect(rect, 18, 14, 28, 8), _warning);
                DrawRect(TileLocalRect(rect, 23, 28, 18, 5), new Color("#101820"));
                DrawRect(TileLocalRect(rect, 29, 22, 6, 18), new Color("#b9ddff"));
                break;
        }
    }

    private void DrawRoadFallback(Rect2 rect, TileCoord coord)
    {
        var hasNorth = IsRoadConnection(coord with { Y = coord.Y - 1 });
        var hasSouth = IsRoadConnection(coord with { Y = coord.Y + 1 });
        var hasWest = IsRoadConnection(coord with { X = coord.X - 1 });
        var hasEast = IsRoadConnection(coord with { X = coord.X + 1 });
        if (!hasNorth && !hasSouth && !hasWest && !hasEast)
        {
            hasWest = true;
            hasEast = true;
        }

        var riverNorth = IsTerrain(coord with { Y = coord.Y - 1 }, TerrainType.River);
        var riverSouth = IsTerrain(coord with { Y = coord.Y + 1 }, TerrainType.River);
        var riverWest = IsTerrain(coord with { X = coord.X - 1 }, TerrainType.River);
        var riverEast = IsTerrain(coord with { X = coord.X + 1 }, TerrainType.River);
        if (riverNorth || riverSouth)
        {
            DrawPathSegments(rect, riverNorth, riverSouth, false, false, new Color("#183c62"), new Color("#4fc3ff"), 24, 18);
        }

        if (riverWest || riverEast)
        {
            DrawPathSegments(rect, false, false, riverWest, riverEast, new Color("#183c62"), new Color("#4fc3ff"), 24, 18);
        }

        DrawPathSegments(rect, hasNorth, hasSouth, hasWest, hasEast, new Color("#d2b16e"), new Color("#f0ce84"), 30, 20);
        DrawRect(TileLocalRect(rect, 28, 30, 8, 4), new Color("#8a7147"));
    }

    private void DrawRiverFallback(Rect2 rect, TileCoord coord)
    {
        var hasNorth = IsTerrain(coord with { Y = coord.Y - 1 }, TerrainType.River);
        var hasSouth = IsTerrain(coord with { Y = coord.Y + 1 }, TerrainType.River);
        var hasWest = IsTerrain(coord with { X = coord.X - 1 }, TerrainType.River);
        var hasEast = IsTerrain(coord with { X = coord.X + 1 }, TerrainType.River);
        if (!hasNorth && !hasSouth && !hasWest && !hasEast)
        {
            hasNorth = true;
            hasSouth = true;
        }

        DrawPathSegments(rect, hasNorth, hasSouth, hasWest, hasEast, new Color("#183c62"), new Color("#4fc3ff"), 36, 24);
        DrawRect(TileLocalRect(rect, 12, 27, 18, 4), new Color("#7de2ff"));
        DrawRect(TileLocalRect(rect, 38, 40, 16, 4), new Color("#6fd6ff"));
    }

    private void DrawPathSegments(Rect2 rect, bool north, bool south, bool west, bool east, Color fill, Color highlight, int span, int highlightSpan)
    {
        var offset = (SpriteSize - span) / 2;
        DrawRect(TileLocalRect(rect, offset, offset, span, span), fill);
        if (north)
        {
            DrawRect(TileLocalRect(rect, offset, 0, span, offset + span), fill);
        }

        if (south)
        {
            DrawRect(TileLocalRect(rect, offset, offset, span, SpriteSize - offset), fill);
        }

        if (west)
        {
            DrawRect(TileLocalRect(rect, 0, offset, offset + span, span), fill);
        }

        if (east)
        {
            DrawRect(TileLocalRect(rect, offset, offset, SpriteSize - offset, span), fill);
        }

        var highlightOffset = (SpriteSize - highlightSpan) / 2;
        if (north || south)
        {
            DrawRect(TileLocalRect(rect, highlightOffset, 0, 4, SpriteSize), highlight);
        }

        if (west || east)
        {
            DrawRect(TileLocalRect(rect, 0, highlightOffset, SpriteSize, 4), highlight);
        }
    }

    private bool IsRoadConnection(TileCoord coord) =>
        _state.Contains(coord) && _state.GetTerrain(coord) is TerrainType.Road or TerrainType.Hq or TerrainType.Workshop;

    private static Rect2 TileLocalRect(Rect2 rect, float x, float y, float width, float height) => new(
        rect.Position + new Vector2(rect.Size.X * x / SpriteSize, rect.Size.Y * y / SpriteSize),
        new Vector2(rect.Size.X * width / SpriteSize, rect.Size.Y * height / SpriteSize));

    private void DrawTerrainSprite(TerrainType terrain, Rect2 destination, TileCoord coord)
    {
        if (HasPathSpriteAtlas() && terrain is TerrainType.Road or TerrainType.River)
        {
            var pathSource = PathSpriteSource(terrain, coord);
            DrawTextureRectRegion(_pathSprites, destination, pathSource);
            return;
        }

        if (_terrainSprites is null || terrain is TerrainType.Road or TerrainType.River)
        {
            DrawFallbackTerrainPattern(terrain, destination, coord);
            return;
        }

        var source = new Rect2(TerrainSpriteIndex(terrain, coord) * SpriteSize, 0, SpriteSize, SpriteSize);
        DrawTextureRectRegion(_terrainSprites, destination, source);
    }

    private Rect2 PathSpriteSource(TerrainType terrain, TileCoord coord)
    {
        var mask = terrain == TerrainType.Road ? RoadConnectionMask(coord) : RiverConnectionMask(coord);
        var effectiveMask = terrain == TerrainType.Road && mask == 0 ? 10 : terrain == TerrainType.River && mask == 0 ? 5 : mask;
        var row = terrain == TerrainType.Road ? RoadPathSpriteRow(coord, effectiveMask) : 1;
        return new Rect2(effectiveMask * SpriteSize, row * SpriteSize, SpriteSize, SpriteSize);
    }

    private bool HasPathSpriteAtlas() =>
        _pathSprites is not null
        && _pathSprites.GetWidth() >= SpriteSize * 16
        && _pathSprites.GetHeight() >= SpriteSize * 4;

    private int RoadPathSpriteRow(TileCoord coord, int roadMask)
    {
        var riverNorth = IsTerrain(coord with { Y = coord.Y - 1 }, TerrainType.River);
        var riverSouth = IsTerrain(coord with { Y = coord.Y + 1 }, TerrainType.River);
        var riverWest = IsTerrain(coord with { X = coord.X - 1 }, TerrainType.River);
        var riverEast = IsTerrain(coord with { X = coord.X + 1 }, TerrainType.River);
        var roadEastWest = (roadMask & 2) != 0 || (roadMask & 8) != 0;
        var roadNorthSouth = (roadMask & 1) != 0 || (roadMask & 4) != 0;
        if ((riverNorth || riverSouth) && roadEastWest)
        {
            return 2;
        }

        if ((riverWest || riverEast) && roadNorthSouth)
        {
            return 3;
        }

        return 0;
    }

    private int RoadConnectionMask(TileCoord coord) =>
        (IsRoadConnection(coord with { Y = coord.Y - 1 }) ? 1 : 0)
        | (IsRoadConnection(coord with { X = coord.X + 1 }) ? 2 : 0)
        | (IsRoadConnection(coord with { Y = coord.Y + 1 }) ? 4 : 0)
        | (IsRoadConnection(coord with { X = coord.X - 1 }) ? 8 : 0);

    private int RiverConnectionMask(TileCoord coord) =>
        (IsTerrain(coord with { Y = coord.Y - 1 }, TerrainType.River) ? 1 : 0)
        | (IsTerrain(coord with { X = coord.X + 1 }, TerrainType.River) ? 2 : 0)
        | (IsTerrain(coord with { Y = coord.Y + 1 }, TerrainType.River) ? 4 : 0)
        | (IsTerrain(coord with { X = coord.X - 1 }, TerrainType.River) ? 8 : 0);

    private static Rect2 BoardRect() => new(BoardOriginX, BoardOriginY, BoardPixelWidth, BoardPixelHeight);

    private void DrawScore(Font font)
    {
        var score = BattleRules.CalculateScore(_state);
        var rect = new Rect2(210, 160, 520, 330);
        DrawRect(new Rect2(rect.Position + new Vector2(10, 10), rect.Size), new Color("#050910bb"));
        DrawRect(rect, new Color("#121923f2"));
        DrawRect(new Rect2(rect.Position, new Vector2(rect.Size.X, 58)), new Color("#21334a"));
        DrawRect(rect, _warning, filled: false, width: 4);
        DrawString(font, rect.Position + new Vector2(32, 42), _state.Outcome == BattleOutcome.PlayerVictory ? "VICTORY" : "DEFEAT", HorizontalAlignment.Left, -1, 28, _warning);
        DrawScoreRow(font, rect.Position + new Vector2(32, 94), "Objective", score.Objective);
        DrawScoreRow(font, rect.Position + new Vector2(32, 132), "Speed", score.Speed);
        DrawScoreRow(font, rect.Position + new Vector2(32, 170), "Technique", score.Technique);
        DrawScoreRow(font, rect.Position + new Vector2(32, 208), "Power", score.Power);
        DrawString(font, rect.Position + new Vector2(32, 274), $"TOTAL {score.Total}", HorizontalAlignment.Left, -1, 26, _warning);
        DrawString(font, rect.Position + new Vector2(278, 274), "Enter/A or R", HorizontalAlignment.Left, -1, 16, _text);
    }

    private void DrawUnits()
    {
        foreach (var unit in _state.Units.Where(unit => unit.IsAlive))
        {
            if (!IsTileVisible(unit.Position))
            {
                continue;
            }

            var hitProgress = HitFlashProgress(unit.Id);
            var rect = TileRect(unit.Position).Grow(CurrentTileSize >= BaseTileSize ? -3 : -2);
            rect.Position += HitRecoilOffset(unit, hitProgress);
            var teamColor = TeamColor(unit.Team);
            var hpBarWidth = Math.Max(14f, rect.Size.X - 8f);
            var baseRect = new Rect2(rect.Position + new Vector2(4, rect.Size.Y - 13), new Vector2(hpBarWidth, 10));
            DrawRect(new Rect2(rect.Position + new Vector2(4, 7), new Vector2(rect.Size.X - 8, rect.Size.Y - 4)), new Color("#0509107f"));
            DrawTeamIdentityFrame(unit, rect, teamColor);
            DrawRect(new Rect2(baseRect.Position + new Vector2(4, 3), baseRect.Size), new Color("#0509109f"));
            DrawRect(baseRect, new Color("#101820"));
            DrawRect(baseRect.Grow(-2), teamColor);
            DrawRect(new Rect2(baseRect.Position + new Vector2(6, 2), new Vector2(baseRect.Size.X - 12, 3)), new Color("#ffffff45"));
            DrawUnitSprite(unit, rect);

            if (BattleRules.IsScoutStranded(_state, unit))
            {
                DrawRect(rect.Grow(2), new Color("#050910"), filled: false, width: 6);
                DrawDashedRect(rect.Grow(1), _warning, 9, 3);
            }

            if (unit.Team == Team.Player && unit.HasMoved)
            {
                var overlayAlpha = unit.HasActed ? 0.5f : 0.28f;
                DrawRect(rect.Grow(-3), new Color(0.62f, 0.67f, 0.72f, overlayAlpha));
                DrawRect(rect.Grow(-2), new Color("#e7edf766"), filled: false, width: 2);
            }

            var profile = BattleRules.GetProfile(unit.Type);
            var displayHp = DisplayHp(unit);
            var hpWidth = Math.Max(4f, hpBarWidth * (displayHp / profile.MaxHp));
            DrawDamageOverlay(unit, rect, profile);
            DrawHitFlash(rect, hitProgress);
            DrawRect(new Rect2(rect.Position + new Vector2(4, rect.Size.Y - 8), new Vector2(hpBarWidth, 5)), new Color("#250f14"));
            DrawRect(new Rect2(rect.Position + new Vector2(4, rect.Size.Y - 8), new Vector2(hpWidth, 5)), HpColor(unit, profile));
        }
    }

    private void EndTurn()
    {
        if (IsCutsceneScreen())
        {
            return;
        }

        if (_state.IsComplete)
        {
            return;
        }

        _selectedUnitId = null;
        _actionMode = false;
        _pendingMoveUndoState = null;
        _pendingMoveUnitId = null;
        var beforeEnemyPhase = CaptureUnitSnapshots();
        if (_state.Turn == 1 && !_state.ScoutRescued)
        {
            AddMessage("Ending turn: red units advance. Scout-7 has one grace phase before they can be hit.");
        }

        ApplyAndReport(BattleCommand.EndTurn());
        ReportEnemyPhase(beforeEnemyPhase);
        QueueRedraw();
    }

    private Dictionary<string, UnitSnapshot> CaptureUnitSnapshots() => _state.Units.ToDictionary(
        unit => unit.Id,
        unit => new UnitSnapshot(unit.Id, unit.Team, unit.Type, unit.Position, unit.Hp, unit.IsAlive),
        StringComparer.Ordinal);

    private string ArenaHintText()
    {
        if (_state.IsComplete)
        {
            return _state.Outcome == BattleOutcome.PlayerVictory ? "Mission complete" : "Mission failed";
        }

        var selectedUnit = SelectedUnit();
        if (selectedUnit is null)
        {
            return "Pick a ready blue unit";
        }

        if (_actionMode && IsCaptureReady(selectedUnit))
        {
            return $"Wait to capture {CaptureObjectiveName(selectedUnit.Position)}";
        }

        return _actionMode ? "Choose attack or wait" : "Blue tiles are legal moves";
    }

    private string CancelPromptText()
    {
        if (_selectedUnitId is null)
        {
            return "B --";
        }

        return _actionMode && _pendingMoveUndoState is not null ? "B UNDO" : "B CANCEL";
    }

    private string CounterHudText(CombatForecast forecast) => forecast.CounterMaximumDamage > 0
        ? $"COUNTER -{forecast.CounterMinimumDamage}-{forecast.CounterMaximumDamage} HP"
        : "NO COUNTER";

    private string CursorHudText()
    {
        var cursorUnit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (cursorUnit is null)
        {
            return $"CURSOR {FormatCoord(_cursor)}";
        }

        var profile = BattleRules.GetProfile(cursorUnit.Type);
        return $"{TeamName(cursorUnit.Team)} {UnitTypeLabel(cursorUnit.Type)} HP {cursorUnit.Hp}/{profile.MaxHp}";
    }

    private string CursorTerrainHudText()
    {
        var terrain = _state.GetTerrain(_cursor);
        if (IsCaptureObjective(_cursor))
        {
            return CaptureObjectiveStatus(_cursor);
        }

        var defense = TerrainDefenseValue(terrain);
        if (terrain == TerrainType.River)
        {
            return "River blocks movement";
        }

        if (terrain == TerrainType.Workshop)
        {
            return "Workshop repairs on Wait";
        }

        return defense > 0 ? $"{terrain} DEF +{defense}" : terrain == TerrainType.Road ? "Road fast" : terrain.ToString();
    }

    private string ForecastTerrainHudText(UnitState attacker, UnitState defender)
    {
        var defenderTerrain = _state.GetTerrain(defender.Position);
        var defense = TerrainDefenseValue(defenderTerrain);
        return defense > 0
            ? $"{defenderTerrain} shields {defender.Type} +{defense}"
            : $"{attacker.Type} into {defender.Type}";
    }

    private ActiveForecast? GetActiveForecast()
    {
        var selectedUnit = SelectedUnit();
        var targetUnit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (selectedUnit is null || targetUnit is null || targetUnit.Team == selectedUnit.Team || selectedUnit.Position.DistanceTo(targetUnit.Position) != 1)
        {
            return null;
        }

        return new ActiveForecast(selectedUnit, targetUnit, BattleRules.GetCombatForecast(_state, selectedUnit, targetUnit));
    }

    private string HqHudText() => IsHqThreatened() ? "HQ DANGER" : "HQ SAFE";

    private bool IsHqThreatened() => _state.Units.Any(unit =>
        unit.IsAlive &&
        unit.Team == Team.Enemy &&
        unit.Position.DistanceTo(_state.PlayerHq) <= 2);

    private bool CanRescueScout(UnitState unit) => unit.Team == Team.Player && unit.Id != _state.ScoutId;

    private Color ModeColor() => _selectedUnitId is null
        ? _warning
        : _actionMode ? _enemy : _player;

    private string ObjectiveHudText()
    {
        if (_state.Outcome == BattleOutcome.PlayerVictory) return "MISSION COMPLETE";
        if (_state.Outcome == BattleOutcome.PlayerDefeat) return "MISSION FAILED";
        if (_state.RelayStation != TileCoord.None || _state.FuelCache != TileCoord.None)
        {
            return CaptureHudText();
        }

        if (_state.RequiresScoutSurvival && !_state.ScoutRescued) return "HOLD HQ + REACH SCOUT";
        return $"M{_state.MissionNumber}/10 ROUT RED";
    }

    private string PrimaryPromptText()
    {
        if (_state.IsComplete)
        {
            return _state.Outcome == BattleOutcome.PlayerVictory ? "A DEBRIEF" : "A RETRY";
        }

        var selectedUnit = SelectedUnit();
        if (selectedUnit is null)
        {
            return "A SELECT";
        }

        var targetUnit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (targetUnit is { Team: Team.Enemy } && selectedUnit.Position.DistanceTo(targetUnit.Position) == 1)
        {
            return "A ATTACK";
        }

        if (_actionMode && _cursor == selectedUnit.Position)
        {
            if (IsCaptureReady(selectedUnit))
            {
                return "A CAPTURE";
            }

            return "A WAIT";
        }

        return _actionMode ? "A ACTION" : "A MOVE";
    }

    private int PrimaryPromptIconIndex()
    {
        if (_state.IsComplete)
        {
            return _state.Outcome == BattleOutcome.PlayerVictory ? 7 : 2;
        }

        var selectedUnit = SelectedUnit();
        if (selectedUnit is null)
        {
            return 9;
        }

        var targetUnit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (targetUnit is { Team: Team.Enemy } && selectedUnit.Position.DistanceTo(targetUnit.Position) == 1)
        {
            return 1;
        }

        if (_actionMode && _cursor == selectedUnit.Position)
        {
            return 2;
        }

        return _actionMode ? 9 : 0;
    }

    private bool IsCaptureReady(UnitState unit) =>
        CanUnitCaptureObjectives(unit) && IsUnsecuredCaptureObjective(unit.Position);

    private UnitState? SelectedUnit() => _selectedUnitId is null
        ? null
        : _state.Units.FirstOrDefault(unit => unit.Id == _selectedUnitId && unit.IsAlive);

    private string ScoutHudText()
    {
        if (_state.RelayStation != TileCoord.None || _state.FuelCache != TileCoord.None)
        {
            return $"RLY {_state.RelayCaptureProgress}/2 FUEL {_state.FuelCaptureProgress}/2";
        }

        if (!_state.RequiresScoutSurvival)
        {
            return $"MISSION {_state.MissionNumber}/10";
        }

        var scout = _state.Units.FirstOrDefault(unit => unit.Id == _state.ScoutId);
        if (scout is not { IsAlive: true })
        {
            return "SCOUT LOST";
        }

        return _state.ScoutRescued ? "SCOUT SAFE" : "SCOUT STRANDED";
    }

    private string CaptureHudText()
    {
        var relay = _state.RelayStation == TileCoord.None || _state.RelaySecured;
        var fuel = _state.FuelCache == TileCoord.None || _state.FuelSecured;
        if (relay && fuel)
        {
            return "OBJECTIVES SECURE";
        }

        if (_state.RelayStation != TileCoord.None && _state.FuelCache != TileCoord.None)
        {
            return "SECURE RELAY + FUEL";
        }

        return "SECURE NODE";
    }

    private string CaptureObjectiveStatus(TileCoord coord)
    {
        if (coord == _state.RelayStation)
        {
            return _state.RelaySecured
                ? "Relay secured"
                : $"Relay {_state.RelayCaptureProgress}/2: Wait to capture";
        }

        if (coord == _state.FuelCache)
        {
            return _state.FuelSecured
                ? "Fuel Cache secured"
                : $"Fuel {_state.FuelCaptureProgress}/2: Wait to capture";
        }

        return string.Empty;
    }

    private string CaptureObjectiveName(TileCoord coord)
    {
        if (coord == _state.RelayStation)
        {
            return "Relay";
        }

        if (coord == _state.FuelCache)
        {
            return "Fuel Cache";
        }

        return "objective";
    }

    private bool IsCaptureObjective(TileCoord coord) =>
        coord == _state.RelayStation || coord == _state.FuelCache;

    private bool IsUnsecuredCaptureObjective(TileCoord coord) =>
        (coord == _state.RelayStation && !_state.RelaySecured) ||
        (coord == _state.FuelCache && !_state.FuelSecured);

    private static bool CanUnitCaptureObjectives(UnitState unit) =>
        unit.Team == Team.Player && unit.Type is UnitType.Infantry or UnitType.Engineer or UnitType.FieldRig;

    private static string ObjectiveMarkerText(string label, int progress, bool secured) => secured
        ? $"{label} OK"
        : $"{label} {progress}/2";

    private string SelectedHudText()
    {
        var selectedUnit = SelectedUnit();
        if (selectedUnit is null)
        {
            return "No unit selected";
        }

        return $"{selectedUnit.Id} {(_actionMode ? "ACT" : "MOVE")}";
    }

    private static int TerrainDefenseValue(TerrainType terrain) => terrain switch
    {
        TerrainType.Cover => 2,
        TerrainType.Hq => 3,
        _ => 0
    };

    private Color TerrainHudColor(TerrainType terrain) => terrain switch
    {
        TerrainType.Cover => _player,
        TerrainType.Hq => _warning,
        TerrainType.Road => _road,
        TerrainType.River => new Color("#4fc3ff"),
        TerrainType.Workshop => _warning,
        _ => _text
    };

    private static string TerrainValueText(TerrainType terrain) => terrain switch
    {
        TerrainType.Cover => "Cover reduces incoming damage",
        TerrainType.Hq => "HQ cover is strongest",
        TerrainType.Road => "Road supports fast movement",
        TerrainType.Ridge => "Ridge blocks movement",
        TerrainType.River => "River blocks movement; cross on bridge roads",
        TerrainType.Workshop => "Workshop: Wait here to repair 3 HP",
        _ => "Plain terrain has no defense bonus"
    };

    private void ApplyCombatFeedback(BattleCommand command, IReadOnlyDictionary<string, UnitSnapshot> beforeCommand)
    {
        foreach (var unit in _state.Units)
        {
            if (!beforeCommand.TryGetValue(unit.Id, out var before) || !before.IsAlive || unit.Hp >= before.Hp)
            {
                continue;
            }

            var damage = before.Hp - unit.Hp;
            var isCounterDamage = string.Equals(unit.Id, command.UnitId, StringComparison.Ordinal);
            RegisterDamageFeedback(before, unit.Id, damage, isCounterDamage, !unit.IsAlive);
        }
    }

    private void RegisterDamageFeedback(UnitSnapshot before, string unitId, int damage, bool isCounterDamage, bool isDestroyed)
    {
        _damagePopups.Add(new DamagePopup
        {
            Amount = damage,
            Coord = before.Position,
            IsCounterDamage = isCounterDamage,
            IsDestroyed = isDestroyed
        });

        if (isDestroyed)
        {
            _displayHpByUnitId.Remove(unitId);
            _hitFlashByUnitId.Remove(unitId);
            return;
        }

        _displayHpByUnitId[unitId] = before.Hp;
        _hitFlashByUnitId[unitId] = HitFlashDuration;
    }

    private string DescribeUnit(UnitSnapshot snapshot) => $"{TeamName(snapshot.Team)} {snapshot.Type} {snapshot.Id}";

    private string DescribeUnit(UnitState unit) => $"{TeamName(unit.Team)} {unit.Type} {unit.Id}";

    private void ReportEnemyPhase(IReadOnlyDictionary<string, UnitSnapshot> beforeEnemyPhase)
    {
        var phaseMessages = new List<string>();
        foreach (var unit in _state.Units.OrderBy(unit => unit.Id, StringComparer.Ordinal))
        {
            if (!beforeEnemyPhase.TryGetValue(unit.Id, out var before))
            {
                continue;
            }

            if (before.IsAlive && !unit.IsAlive)
            {
                phaseMessages.Add($"{DescribeUnit(before)} was destroyed.");
                continue;
            }

            if (!before.IsAlive || !unit.IsAlive)
            {
                continue;
            }

            if (before.Team == Team.Enemy && before.Position != unit.Position)
            {
                phaseMessages.Add($"{unit.Id} moved {FormatCoord(before.Position)} to {FormatCoord(unit.Position)}.");
            }

            if (before.Hp != unit.Hp)
            {
                var damage = before.Hp - unit.Hp;
                var verb = damage > 0 ? "lost" : "gained";
                phaseMessages.Add($"{DescribeUnit(unit)} {verb} {Math.Abs(damage)} HP ({before.Hp}->{unit.Hp}).");
            }
        }

        if (phaseMessages.Count == 0)
        {
            AddMessage("Enemy phase: red units held position. No damage dealt.");
            return;
        }

        AddMessage("Enemy phase recap:");
        foreach (var message in phaseMessages.Take(4).Reverse())
        {
            AddMessage(message);
        }
    }

    private string DefeatReasonText()
    {
        if (_state.Units.Any(unit => unit.IsAlive && unit.Team == Team.Enemy && unit.Position == _state.PlayerHq))
        {
            return "Defeat reason: a red unit reached the Kestrel staging HQ.";
        }

        var scout = _state.Units.FirstOrDefault(unit => unit.Id == _state.ScoutId);
        if (_state.RequiresScoutSurvival && scout is not { IsAlive: true })
        {
            return "Defeat reason: Scout-7 was destroyed before extraction.";
        }

        return $"Defeat reason: {_state.DefeatLine}";
    }

    private void DrawModeBanner(Font font, ref int yPosition)
    {
        var bannerColor = _selectedUnitId is null
            ? _warning
            : _actionMode ? _enemy : _player;
        DrawRect(new Rect2(PanelX + 24, yPosition - 16, PanelContentWidth, 30), new Color("#0d1420"));
        DrawRect(new Rect2(PanelX + 24, yPosition - 16, 8, 30), bannerColor);
        DrawRect(new Rect2(PanelX + 24, yPosition - 16, PanelContentWidth, 30), bannerColor, filled: false, width: 2);
        DrawString(font, new Vector2(PanelX + 36, yPosition + 4), ModeTitle(), HorizontalAlignment.Left, PanelContentWidth - 24, 15, bannerColor);
        yPosition += 32;
    }

    private int DrawWrappedLine(Font font, string text, int yPosition, int fontSize, Color color, int maxChars, int maxLines)
    {
        foreach (var line in WrapText(text, maxChars).Take(maxLines))
        {
            DrawString(font, new Vector2(PanelX + 24, yPosition), line, HorizontalAlignment.Left, PanelContentWidth, fontSize, color);
            yPosition += fontSize + 7;
        }

        return yPosition;
    }

    private void DrawLegend(Font font, ref int yPosition)
    {
        DrawLegendSwatch(new Vector2(PanelX + 24, yPosition - 12), _player, _playerDark);
        DrawString(font, new Vector2(PanelX + 46, yPosition), "Cyan frame = blue units", HorizontalAlignment.Left, 145, 14, _text);
        DrawLegendSwatch(new Vector2(PanelX + 194, yPosition - 12), _enemy, _enemyDark);
        DrawString(font, new Vector2(PanelX + 216, yPosition), "Red frame = raiders", HorizontalAlignment.Left, 125, 14, _text);
        yPosition += 22;
        DrawRect(new Rect2(PanelX + 24, yPosition - 12, 16, 10), _warning);
        DrawRect(new Rect2(PanelX + 27, yPosition - 9, 10, 4), new Color("#fff0a0"));
        DrawString(font, new Vector2(PanelX + 46, yPosition), "Yellow ring = stranded Scout-7", HorizontalAlignment.Left, 290, 14, _text);
        yPosition += 22;
        yPosition = DrawWrappedLine(font, "Shapes: soldier = infantry/tech, tank = armor, wedge = scout/striker, wrench = support, lance = anti-armor.", yPosition, 14, _text, 41, 2);
    }

    private void DrawLegendSwatch(Vector2 position, Color light, Color dark)
    {
        DrawRect(new Rect2(position, new Vector2(16, 14)), dark);
        DrawRect(new Rect2(position + new Vector2(3, 3), new Vector2(10, 8)), light);
    }

    private void DrawTeamIdentityFrame(UnitState unit, Rect2 rect, Color teamColor)
    {
        var dark = TeamDarkColor(unit.Team);
        DrawRect(rect.Grow(1), new Color("#050910"), filled: false, width: 6);
        DrawRect(rect.Grow(-1), teamColor, filled: false, width: 3);

        var tab = new Rect2(rect.Position + new Vector2(2, 2), new Vector2(19, 16));
        DrawRect(new Rect2(tab.Position + new Vector2(2, 2), tab.Size), new Color("#050910aa"));
        DrawRect(tab, dark);
        DrawRect(tab.Grow(-2), teamColor);
        DrawString(GetThemeDefaultFont(), tab.Position + new Vector2(5, 13), unit.Team == Team.Player ? "B" : "R", HorizontalAlignment.Left, 10, 12, new Color("#050910"));

        if (unit.Team == Team.Player)
        {
            DrawRect(new Rect2(rect.Position + new Vector2(4, 20), new Vector2(4, 20)), teamColor);
            DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - 8, 20), new Vector2(4, 20)), teamColor);
            return;
        }

        DrawRect(new Rect2(rect.Position + new Vector2(8, 8), new Vector2(10, 4)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - 18, 8), new Vector2(10, 4)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(8, rect.Size.Y - 22), new Vector2(10, 4)), _warning);
        DrawRect(new Rect2(rect.Position + new Vector2(rect.Size.X - 18, rect.Size.Y - 22), new Vector2(10, 4)), _warning);
    }

    private void DrawCombatFeedback()
    {
        var font = GetThemeDefaultFont();
        foreach (var popup in _damagePopups)
        {
            if (!IsTileVisible(popup.Coord))
            {
                continue;
            }

            var progress = Math.Clamp(popup.Age / DamagePopupDuration, 0f, 1f);
            var alpha = progress < 0.72f ? 1f : 1f - ((progress - 0.72f) / 0.28f);
            var tileRect = TileRect(popup.Coord);
            var xOffset = popup.IsCounterDamage ? 4 : 15;
            var yOffset = 18 - (28 * progress);
            var label = popup.IsCounterDamage ? $"RET -{popup.Amount}" : $"-{popup.Amount}";
            if (popup.IsDestroyed)
            {
                label += " KO";
                DrawDestroyedBurst(tileRect, progress, alpha);
            }

            var color = popup.IsCounterDamage
                ? FadeColor(_warning, alpha)
                : FadeColor(new Color("#ff766e"), alpha);
            DrawOutlinedString(font, tileRect.Position + new Vector2(xOffset, yOffset), label, 16, color);
        }
    }

    private void DrawDamageOverlay(UnitState unit, Rect2 rect, UnitProfile profile)
    {
        var hpRatio = unit.Hp / (float)profile.MaxHp;
        if (hpRatio > 0.6f)
        {
            return;
        }

        DrawRect(rect.Grow(-7), new Color(hpRatio <= 0.33f ? "#3d101a66" : "#2b10184d"));
        DrawRect(new Rect2(rect.Position + new Vector2(14, 17), new Vector2(14, 3)), new Color("#ffd1bd99"));
        DrawRect(new Rect2(rect.Position + new Vector2(36, 28), new Vector2(11, 3)), new Color("#05091099"));
        DrawRect(new Rect2(rect.Position + new Vector2(22, 39), new Vector2(18, 3)), new Color("#ffd1bd80"));

        if (hpRatio > 0.33f)
        {
            return;
        }

        DrawRect(new Rect2(rect.Position + new Vector2(9, 10), new Vector2(8, 5)), new Color("#050910aa"));
        DrawRect(new Rect2(rect.Position + new Vector2(47, 15), new Vector2(7, 4)), new Color("#05091099"));
        DrawRect(new Rect2(rect.Position + new Vector2(41, 7), new Vector2(5, 3)), new Color("#aeb8c266"));
    }

    private void DrawDestroyedBurst(Rect2 tileRect, float progress, float alpha)
    {
        if (progress > 0.68f)
        {
            return;
        }

        var burst = Math.Clamp(progress / 0.68f, 0f, 1f);
        var center = tileRect.Position + new Vector2(tileRect.Size.X / 2f, tileRect.Size.Y / 2f);
        var color = FadeColor(new Color("#f6c85f"), alpha * (1f - burst));
        var dark = FadeColor(new Color("#050910"), alpha * (1f - burst));
        var radius = 10f + (18f * burst);
        DrawRect(new Rect2(center + new Vector2(-radius, -4), new Vector2(10, 8)), color);
        DrawRect(new Rect2(center + new Vector2(radius - 10, -4), new Vector2(10, 8)), color);
        DrawRect(new Rect2(center + new Vector2(-5, -radius), new Vector2(10, 8)), color);
        DrawRect(new Rect2(center + new Vector2(-5, radius - 8), new Vector2(10, 8)), dark);
    }

    private void DrawHitFlash(Rect2 rect, float hitProgress)
    {
        if (hitProgress <= 0f)
        {
            return;
        }

        DrawRect(rect.Grow(-6), new Color(1f, 1f, 1f, 0.38f * hitProgress));
    }

    private void DrawOutlinedString(Font font, Vector2 position, string text, int fontSize, Color color)
    {
        var outline = FadeColor(new Color("#050910"), color.A);
        DrawString(font, position + new Vector2(1, 0), text, HorizontalAlignment.Left, -1, fontSize, outline);
        DrawString(font, position + new Vector2(-1, 0), text, HorizontalAlignment.Left, -1, fontSize, outline);
        DrawString(font, position + new Vector2(0, 1), text, HorizontalAlignment.Left, -1, fontSize, outline);
        DrawString(font, position + new Vector2(0, -1), text, HorizontalAlignment.Left, -1, fontSize, outline);
        DrawString(font, position, text, HorizontalAlignment.Left, -1, fontSize, color);
    }

    private void DrawScoreRow(Font font, Vector2 position, string label, int value)
    {
        DrawString(font, position, label, HorizontalAlignment.Left, 130, 18, _text);
        DrawRect(new Rect2(position + new Vector2(150, -15), new Vector2(210, 18)), new Color("#070b12"));
        DrawRect(new Rect2(position + new Vector2(152, -13), new Vector2(Math.Clamp(value * 2, 0, 200), 14)), value >= 80 ? new Color("#8be06f") : _warning);
        DrawString(font, position + new Vector2(372, 0), value.ToString(), HorizontalAlignment.Right, 60, 18, _text);
    }

    private void DrawSectionHeader(Font font, string label, ref int yPosition)
    {
        DrawRect(new Rect2(PanelX + 24, yPosition - 13, PanelContentWidth, 19), new Color("#0d1420"));
        DrawRect(new Rect2(PanelX + 24, yPosition - 13, 5, 19), _warning);
        DrawString(font, new Vector2(PanelX + 38, yPosition + 2), label, HorizontalAlignment.Left, PanelContentWidth - 16, 12, _warning);
        yPosition += 24;
    }

    private void DrawTargetingChip(UnitState selectedUnit)
    {
        var targetUnit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (targetUnit is null || targetUnit.Team == selectedUnit.Team || selectedUnit.Position.DistanceTo(targetUnit.Position) != 1)
        {
            return;
        }

        var forecast = BattleRules.GetCombatForecast(_state, selectedUnit, targetUnit);
        var font = GetThemeDefaultFont();
        var rect = TileRect(targetUnit.Position).Grow(-7);
        var chipPosition = rect.Position + new Vector2(7, 9);
        DrawRect(new Rect2(chipPosition + new Vector2(-4, -13), new Vector2(49, 17)), new Color("#050910dd"));
        DrawString(font, chipPosition, $"-{forecast.MinimumDamage}-{forecast.MaximumDamage}", HorizontalAlignment.Left, 42, 12, new Color("#ffbeb4"));

        if (forecast.CounterMaximumDamage <= 0)
        {
            return;
        }

        var counterPosition = chipPosition + new Vector2(0, 17);
        DrawRect(new Rect2(counterPosition + new Vector2(-4, -13), new Vector2(55, 17)), new Color("#050910dd"));
        DrawString(font, counterPosition, $"RET {forecast.CounterMinimumDamage}-{forecast.CounterMaximumDamage}", HorizontalAlignment.Left, 50, 12, _warning);
    }

    private void DrawUnitSprite(UnitState unit, Rect2 rect)
    {
        if (_unitSprites is null)
        {
            DrawFallbackUnitSprite(unit, rect);
            return;
        }

        var hasFactionRows = _unitSprites.GetHeight() >= SpriteSize * 2;
        var row = hasFactionRows && unit.Team == Team.Enemy ? 1 : 0;
        var source = new Rect2(UnitSpriteIndex(unit.Type) * SpriteSize, row * SpriteSize, SpriteSize, SpriteSize);
        if (hasFactionRows)
        {
            DrawTextureRectRegion(_unitSprites, rect, source);
            return;
        }

        DrawTextureRectRegion(_unitSprites, rect, source, TeamSpriteTint(unit.Team));
    }

    private void DrawUiIcon(Rect2 destination, int iconIndex)
    {
        if (_uiIconSprites is not null)
        {
            DrawTextureRectRegion(_uiIconSprites, destination, new Rect2(iconIndex * SpriteSize, 0, SpriteSize, SpriteSize));
        }
    }

    private void MoveCursor(TileCoord coord)
    {
        if (!_state.Contains(coord))
        {
            return;
        }

        _cursor = coord;
        UpdateViewOrigin();
        QueueRedraw();
    }

    private void DrawViewportBadge()
    {
        if (_state.Width <= VisibleBoardColumns && _state.Height <= VisibleBoardRows)
        {
            return;
        }

        var font = GetThemeDefaultFont();
        var rect = new Rect2(BoardOriginX + BoardPixelWidth - 128, BoardOriginY + 8, 118, 24);
        DrawRect(new Rect2(rect.Position + new Vector2(2, 2), rect.Size), new Color("#05091099"));
        DrawRect(rect, new Color("#101820dd"));
        DrawRect(rect, _warning, filled: false, width: 1);
        DrawString(font, rect.Position + new Vector2(8, 17), $"MAP {_viewOrigin.X + 1}-{Math.Min(_state.Width, _viewOrigin.X + VisibleBoardColumns)}/{_state.Width}", HorizontalAlignment.Left, 100, 12, _warning);
    }

    private static string FormatCoord(TileCoord coord) => $"{coord.X},{coord.Y}";

    private static Texture2D? LoadSpriteSheet(string resourcePath)
    {
        var image = Image.LoadFromFile(resourcePath);
        return image is null || image.IsEmpty()
            ? null
            : ImageTexture.CreateFromImage(image);
    }

    private void DrawFallbackUnitSprite(UnitState unit, Rect2 rect)
    {
        var dark = unit.Team == Team.Player ? _playerDark : _enemyDark;
        var light = unit.Team == Team.Player ? new Color("#b9ddff") : new Color("#ffc1a8");
        var outline = new Color("#071018");

        switch (unit.Type)
        {
            case UnitType.Infantry:
                DrawRect(new Rect2(rect.Position + new Vector2(13, 4), new Vector2(18, 10)), light);
                DrawRect(new Rect2(rect.Position + new Vector2(14, 16), new Vector2(16, 18)), dark);
                DrawRect(new Rect2(rect.Position + new Vector2(8, 34), new Vector2(10, 8)), outline);
                DrawRect(new Rect2(rect.Position + new Vector2(26, 34), new Vector2(10, 8)), outline);
                break;
            case UnitType.Armor:
                DrawRect(new Rect2(rect.Position + new Vector2(4, 24), new Vector2(44, 16)), outline);
                DrawRect(new Rect2(rect.Position + new Vector2(9, 18), new Vector2(26, 13)), light);
                DrawRect(new Rect2(rect.Position + new Vector2(33, 21), new Vector2(18, 5)), dark);
                break;
            case UnitType.Scout:
                DrawRect(new Rect2(rect.Position + new Vector2(8, 25), new Vector2(36, 13)), outline);
                DrawRect(new Rect2(rect.Position + new Vector2(15, 14), new Vector2(23, 14)), light);
                DrawRect(new Rect2(rect.Position + new Vector2(11, 39), new Vector2(9, 9)), dark);
                DrawRect(new Rect2(rect.Position + new Vector2(33, 39), new Vector2(9, 9)), dark);
                break;
            default:
                DrawRect(new Rect2(rect.Position + new Vector2(8, 24), new Vector2(39, 18)), outline);
                DrawRect(new Rect2(rect.Position + new Vector2(14, 15), new Vector2(24, 16)), light);
                DrawRect(new Rect2(rect.Position + new Vector2(17, 42), new Vector2(26, 6)), dark);
                break;
        }
    }

    private static Color HpColor(UnitState unit, UnitProfile profile)
    {
        var hpRatio = unit.Hp / (float)profile.MaxHp;
        if (hpRatio <= 0.33f)
        {
            return new Color("#f26b5e");
        }

        if (hpRatio <= 0.6f)
        {
            return new Color("#f6c85f");
        }

        return new Color("#8be06f");
    }

    private Color TeamColor(Team team) => team == Team.Player ? _player : _enemy;

    private Color TeamDarkColor(Team team) => team == Team.Player ? _playerDark : _enemyDark;

    private static Color TeamSpriteTint(Team team) => team == Team.Player
        ? new Color(0.78f, 1.08f, 1.34f, 1f)
        : new Color(1.42f, 0.52f, 0.38f, 1f);

    private string ObjectiveText()
    {
        if (_state.Outcome == BattleOutcome.PlayerVictory) return "Mission complete";
        if (_state.Outcome == BattleOutcome.PlayerDefeat) return "Mission failed";
        if (_state.RelayStation != TileCoord.None || _state.FuelCache != TileCoord.None)
        {
            return CaptureHudText() == "OBJECTIVES SECURE"
                ? "Objectives secure, defeat raiders"
                : _state.ObjectiveSummary;
        }

        if (_state.RequiresScoutSurvival && !_state.ScoutRescued) return "Hold HQ, reach Scout-7";
        return _state.ObjectiveSummary;
    }

    private string ModeInstructionText()
    {
        if (_autoplayEnabled)
        {
            return $"AI playtest running. {_autoplayActionsThisMission} automated actions this mission.";
        }

        if (_state.IsComplete)
        {
            return _state.Outcome == BattleOutcome.PlayerVictory
                ? "Mission ended. Press Enter/A to continue to debrief."
                : "Mission ended. Press Enter/A or R to restart.";
        }

        if (_selectedUnitId is null)
        {
            return "Select mode: move cursor to a blue ready unit, then press Enter/A.";
        }

        if (!_actionMode)
        {
            return "Move mode: blue squares are legal moves. Press Enter/A on one to move.";
        }

        var selectedUnit = SelectedUnit();
        if (selectedUnit is not null && IsCaptureReady(selectedUnit))
        {
            return $"Action mode: press Enter/A on this unit to Wait and capture {CaptureObjectiveName(selectedUnit.Position)}.";
        }

        return "Action mode: choose an adjacent red unit to attack, or press Enter/A on your unit to wait.";
    }

    private string ModeTitle()
    {
        if (_state.IsComplete)
        {
            return _state.Outcome == BattleOutcome.PlayerVictory ? "MISSION COMPLETE" : "MISSION FAILED";
        }

        if (_selectedUnitId is null)
        {
            return "SELECT MODE";
        }

        return _actionMode ? "ACTION MODE" : "MOVE MODE";
    }

    private void ResetMission()
    {
        _autoplayCommands.Clear();
        _autoplayActionsThisMission = 0;
        StartMission(_missionNumber);
    }

    private void RetryOrAdvanceCompleteMission()
    {
        if (_state.IsComplete && _state.Outcome == BattleOutcome.PlayerVictory)
        {
            ShowMissionOutro();
            return;
        }

        ResetMission();
    }

    private void StartMission(int missionNumber)
    {
        _missionNumber = missionNumber;
        _screen = CampaignScreen.MissionBattle;
        _state = CampaignMissionFactory.Create(missionNumber);
        _cursor = FirstReadyPlayerPosition();
        UpdateViewOrigin();
        _selectedUnitId = null;
        _actionMode = false;
        _pendingMoveUndoState = null;
        _pendingMoveUnitId = null;
        _autoplayCommands.Clear();
        _autoplayActionsThisMission = 0;
        ClearCombatFeedback();
        _messages.Clear();
        AddMessage(_state.RescueInstruction);
        AddMessage($"Briefing: {_state.ObjectiveSummary}");
        AddMessage($"Mission {_state.MissionNumber}/10: {_state.MissionTitle}.");
        QueueRedraw();
    }

    private void ShowMissionIntro(int missionNumber)
    {
        _missionNumber = missionNumber;
        _state = CampaignMissionFactory.Create(missionNumber);
        _viewOrigin = new TileCoord(0, 0);
        _screen = CampaignScreen.MissionIntro;
        _selectedUnitId = null;
        _actionMode = false;
        _pendingMoveUndoState = null;
        _pendingMoveUnitId = null;
        _autoplayCommands.Clear();
        _autoplayActionsThisMission = 0;
        ClearCombatFeedback();
        _messages.Clear();
        QueueRedraw();
    }

    private void ShowMissionOutro()
    {
        _screen = CampaignScreen.MissionOutro;
        _selectedUnitId = null;
        _actionMode = false;
        _pendingMoveUndoState = null;
        _pendingMoveUnitId = null;
        ClearCombatFeedback();
        QueueRedraw();
    }

    private bool IsCutsceneScreen() => _screen is CampaignScreen.MissionIntro or CampaignScreen.MissionOutro or CampaignScreen.CampaignComplete;

    private void AdvanceCutscene()
    {
        if (_screen == CampaignScreen.MissionIntro)
        {
            StartMission(_missionNumber);
            return;
        }

        if (_screen == CampaignScreen.MissionOutro)
        {
            if (_missionNumber >= CampaignMissionCatalog.FinalMissionNumber)
            {
                _screen = CampaignScreen.CampaignComplete;
                QueueRedraw();
                return;
            }

            ShowMissionIntro(_missionNumber + 1);
        }
    }

    private void DrawCutsceneScreen()
    {
        var font = GetThemeDefaultFont();
        var isOutro = _screen == CampaignScreen.MissionOutro;
        var isComplete = _screen == CampaignScreen.CampaignComplete;
        var image = _missionNumber == 2 ? _mission2Concept : _mission1Concept;
        var title = isComplete
            ? "ACT 1 COMPLETE"
            : $"MISSION {_state.MissionNumber}: {_state.MissionTitle.ToUpperInvariant()}";
        var subtitle = isComplete
            ? "The first ten operations are linked, cleared, and ready to open Act 2."
            : isOutro ? "Debrief" : _state.MissionSubtitle;
        var line = isComplete
            ? "Sloane: We have the refinery node and the broadcast. Now the basin answers back."
            : isOutro ? _state.VictoryLine : _state.IntroLine;
        var bodyLines = CutsceneBodyLines(isComplete, isOutro);

        DrawRect(new Rect2(Vector2.Zero, Size), new Color("#050910"));
        if (image is not null && _missionNumber <= 2)
        {
            DrawTextureRect(image, FitTextureRect(image, new Rect2(0, 0, Size.X, Size.Y)), false);
            DrawRect(new Rect2(Vector2.Zero, Size), new Color("#05091055"));
        }
        else
        {
            DrawProceduralMissionScene(isOutro);
        }

        DrawRect(new Rect2(0, 0, Size.X, 94), new Color("#05091099"));
        DrawString(font, new Vector2(48, 58), title, HorizontalAlignment.Left, Size.X - 96, 30, _warning);
        DrawString(font, new Vector2(52, 88), subtitle, HorizontalAlignment.Left, Size.X - 104, 18, _text);

        var dialogueRect = new Rect2(62, Size.Y - 178, Size.X - 124, 112);
        DrawRect(new Rect2(dialogueRect.Position + new Vector2(8, 8), dialogueRect.Size), new Color("#050910aa"));
        DrawRect(dialogueRect, new Color("#101820ee"));
        DrawRect(dialogueRect, _warning, filled: false, width: 3);
        var textInset = 26;
        if (_commanderPortrait is not null)
        {
            var portraitRect = new Rect2(dialogueRect.Position + new Vector2(20, 14), new Vector2(84, 84));
            DrawRect(new Rect2(portraitRect.Position + new Vector2(4, 4), portraitRect.Size), new Color("#050910aa"));
            DrawRect(portraitRect, new Color("#1a2635"));
            DrawTextureRect(_commanderPortrait, FitTextureRectContain(_commanderPortrait, portraitRect.Grow(-4)), false);
            DrawRect(portraitRect, _warning, filled: false, width: 2);
            textInset = 124;
        }

        DrawString(font, dialogueRect.Position + new Vector2(textInset, 28), line, HorizontalAlignment.Left, dialogueRect.Size.X - textInset - 26, 18, _text);
        var bodyY = 52;
        foreach (var bodyLine in bodyLines.Take(2))
        {
            DrawString(font, dialogueRect.Position + new Vector2(textInset, bodyY), bodyLine, HorizontalAlignment.Left, dialogueRect.Size.X - textInset - 250, 15, _text);
            bodyY += 18;
        }

        var prompt = isComplete ? "Campaign ready" : isOutro ? "Enter/A: continue" : "Enter/A: begin";
        DrawString(font, dialogueRect.Position + new Vector2(dialogueRect.Size.X - 230, 88), prompt, HorizontalAlignment.Right, 200, 18, _warning);
    }

    private IReadOnlyList<string> CutsceneBodyLines(bool isComplete, bool isOutro)
    {
        if (isComplete)
        {
            return ["Act 1 is clear: the refinery proof is public, and Orison can no longer frame this as a local accident."];
        }

        return (_state.MissionNumber, isOutro) switch
        {
            (1, false) => ["Goal: hold the staging HQ, reach Scout-7, then rout the first raider screen.", "Why it matters: Scout-7's recorder is the first clean evidence that the attack was planned."],
            (1, true) => ["Scout-7 is back inside the perimeter. Kestrel has proof the contact was organized, not a random raid.", "Next: secure the relay yard before Orison can erase the logistics trail."],
            (2, false) => ["Goal: capture the Relay and Fuel Cache by waiting twice with Tech or Engineer units, then rout the guards.", "The river crossings split the yard; bridges are the only safe way through."],
            (2, true) => ["The relay is authenticated and the fuel cache is under Kestrel control.", "Next: the convoy road opens, but Orison has already staged an ambush near the pump line."],
            (3, false) => ["Goal: break the ambush along Pump Road and keep the convoy route open.", "Use the bridge chokepoints and workshop repairs to rotate damaged units instead of trading everything at once."],
            (3, true) => ["The convoy is through. Civilians and pump crews made it out before Orison could turn the road into a hostage line.", "Next: capture the depot fabricator so Kestrel can stop fighting from an empty toolbox."],
            (4, false) => ["Goal: cross the river, capture the Depot Node, and clear the fabricator guard.", "The Field Rig and workshop let you sustain a longer fight, but the bridges decide who reaches the depot first."],
            (4, true) => ["The depot fabricator is in Kestrel hands. That means repairs, parts, and proof that Orison was holding public infrastructure off-book.", "Next: the antenna fog lifts just enough to reveal who has been listening."],
            (_, false) => [$"Goal: {_state.ObjectiveSummary}", _state.RescueInstruction],
            (_, true) => ["Mission success changed the local map. Kestrel has more leverage, but Orison's next position is already moving."]
        };
    }

    private void DrawProceduralMissionScene(bool isOutro)
    {
        DrawRect(new Rect2(0, 0, Size.X, Size.Y), new Color(isOutro ? "#24202a" : "#172233"));
        var frame = new Rect2(80, 92, Size.X - 160, 420);
        DrawRect(frame, new Color("#25384f"));
        DrawRect(frame.Grow(-28), new Color("#1b2735"));

        switch (_state.MissionNumber)
        {
            case 3:
                DrawPumpRoadScene(frame, isOutro);
                break;
            case 4:
                DrawDepotScene(frame, isOutro);
                break;
            default:
                DrawGenericMissionScene(frame, isOutro);
                break;
        }
    }

    private void DrawPumpRoadScene(Rect2 frame, bool isOutro)
    {
        var inner = frame.Grow(-44);
        DrawRect(inner, new Color("#385b49"));
        DrawRect(new Rect2(inner.Position + new Vector2(0, inner.Size.Y * 0.44f), new Vector2(inner.Size.X, 54)), _road);
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X * 0.38f, 0), new Vector2(42, inner.Size.Y)), new Color("#183c62"));
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X * 0.33f, inner.Size.Y * 0.42f), new Vector2(110, 68)), new Color("#8f6e3d"));

        for (var index = 0; index < 4; index++)
        {
            var x = inner.Position.X + 90 + (index * 92);
            var y = inner.Position.Y + inner.Size.Y * 0.47f + (index % 2) * 8;
            DrawRect(new Rect2(x, y, 54, 22), isOutro ? _player : new Color("#303947"));
            DrawRect(new Rect2(x + 8, y + 22, 10, 10), new Color("#101820"));
            DrawRect(new Rect2(x + 36, y + 22, 10, 10), new Color("#101820"));
        }

        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X - 190, inner.Size.Y * 0.27f), new Vector2(88, 48)), isOutro ? new Color("#2f6b44") : _enemyDark);
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X - 162, inner.Size.Y * 0.23f), new Vector2(30, 22)), _warning);
    }

    private void DrawDepotScene(Rect2 frame, bool isOutro)
    {
        var inner = frame.Grow(-44);
        DrawRect(inner, new Color("#4b5149"));
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X * 0.42f, 0), new Vector2(48, inner.Size.Y)), new Color("#183c62"));
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X * 0.36f, inner.Size.Y * 0.5f), new Vector2(138, 58)), _road);
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X - 255, 72), new Vector2(180, 116)), isOutro ? _playerDark : new Color("#7a6a3f"));
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X - 218, 42), new Vector2(106, 42)), _warning);
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X - 205, 128), new Vector2(82, 26)), new Color("#101820"));
        DrawRect(new Rect2(inner.Position + new Vector2(94, 196), new Vector2(92, 52)), _playerDark);
        DrawRect(new Rect2(inner.Position + new Vector2(112, 176), new Vector2(54, 26)), _player);
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X - 118, 224), new Vector2(72, 36)), isOutro ? _enemyDark : _enemy);
    }

    private void DrawGenericMissionScene(Rect2 frame, bool isOutro)
    {
        var inner = frame.Grow(-44);
        DrawRect(inner, new Color(isOutro ? "#3f4f39" : "#263e55"));
        DrawRect(new Rect2(inner.Position + new Vector2(36, inner.Size.Y * 0.55f), new Vector2(inner.Size.X - 72, 52)), _road);
        DrawRect(new Rect2(inner.Position + new Vector2(inner.Size.X - 220, 92), new Vector2(150, 96)), isOutro ? _playerDark : _enemyDark);
        DrawRect(new Rect2(inner.Position + new Vector2(112, 148), new Vector2(96, 64)), _playerDark);
        DrawRect(new Rect2(inner.Position + new Vector2(150, 118), new Vector2(48, 28)), _warning);
    }

    private TileCoord FirstReadyPlayerPosition() => _state.Units
        .Where(unit => unit.Team == Team.Player && unit.IsAlive && !BattleRules.IsScoutStranded(_state, unit))
        .OrderBy(unit => unit.Position.X)
        .ThenBy(unit => unit.Position.Y)
        .Select(unit => unit.Position)
        .FirstOrDefault(_state.PlayerHq);

    private static Rect2 FitTextureRect(Texture2D texture, Rect2 bounds)
    {
        var textureSize = texture.GetSize();
        var scale = Math.Max(bounds.Size.X / textureSize.X, bounds.Size.Y / textureSize.Y);
        var size = textureSize * scale;
        return new Rect2(bounds.Position + ((bounds.Size - size) / 2f), size);
    }

    private static Rect2 FitTextureRectContain(Texture2D texture, Rect2 bounds)
    {
        var textureSize = texture.GetSize();
        var scale = Math.Min(bounds.Size.X / textureSize.X, bounds.Size.Y / textureSize.Y);
        var size = textureSize * scale;
        return new Rect2(bounds.Position + ((bounds.Size - size) / 2f), size);
    }

    private Color TileColor(TerrainType terrain) => terrain switch
    {
        TerrainType.Plain => _plain,
        TerrainType.Road => _road,
        TerrainType.Cover => _cover,
        TerrainType.Hq => _hq,
        TerrainType.Ridge => new Color("#202938"),
        TerrainType.River => new Color("#183c62"),
        TerrainType.Workshop => new Color("#7a6a3f"),
        _ => _plain
    };

    private Rect2 TileRect(TileCoord coord) => new(
        BoardOriginX + (coord.X - _viewOrigin.X) * CurrentTileSize,
        BoardOriginY + (coord.Y - _viewOrigin.Y) * CurrentTileSize,
        CurrentTileSize,
        CurrentTileSize);

    private bool IsTileVisible(TileCoord coord) =>
        coord.X >= _viewOrigin.X &&
        coord.Y >= _viewOrigin.Y &&
        coord.X < _viewOrigin.X + VisibleBoardColumns &&
        coord.Y < _viewOrigin.Y + VisibleBoardRows;

    private void UpdateViewOrigin()
    {
        var maxX = Math.Max(0, _state.Width - VisibleBoardColumns);
        var maxY = Math.Max(0, _state.Height - VisibleBoardRows);
        var originX = _viewOrigin.X;
        var originY = _viewOrigin.Y;

        if (_cursor.X < originX + 2)
        {
            originX = _cursor.X - 2;
        }
        else if (_cursor.X >= originX + VisibleBoardColumns - 2)
        {
            originX = _cursor.X - VisibleBoardColumns + 3;
        }

        if (_cursor.Y < originY + 2)
        {
            originY = _cursor.Y - 2;
        }
        else if (_cursor.Y >= originY + VisibleBoardRows - 2)
        {
            originY = _cursor.Y - VisibleBoardRows + 3;
        }

        _viewOrigin = new TileCoord(Math.Clamp(originX, 0, maxX), Math.Clamp(originY, 0, maxY));
    }

    private void ClearCombatFeedback()
    {
        _damagePopups.Clear();
        _displayHpByUnitId.Clear();
        _hitFlashByUnitId.Clear();
    }

    private float DisplayHp(UnitState unit)
    {
        var profile = BattleRules.GetProfile(unit.Type);
        return _displayHpByUnitId.TryGetValue(unit.Id, out var displayHp)
            ? Math.Clamp(displayHp, 0f, profile.MaxHp)
            : unit.Hp;
    }

    private static Color FadeColor(Color color, float alpha) => new(color.R, color.G, color.B, Math.Clamp(alpha, 0f, 1f));

    private float HitFlashProgress(string unitId) => _hitFlashByUnitId.TryGetValue(unitId, out var remaining)
        ? Math.Clamp(remaining / HitFlashDuration, 0f, 1f)
        : 0f;

    private static Vector2 HitRecoilOffset(UnitState unit, float hitProgress)
    {
        if (hitProgress <= 0f)
        {
            return Vector2.Zero;
        }

        var direction = unit.Team == Team.Player ? -1f : 1f;
        var wobble = MathF.Sin(hitProgress * MathF.PI * 3f) * 3f * hitProgress;
        return new Vector2(direction * wobble, -MathF.Abs(wobble) * 0.25f);
    }

    private bool UpdateCombatFeedback(float delta)
    {
        var needsRedraw = _damagePopups.Count > 0 || _displayHpByUnitId.Count > 0 || _hitFlashByUnitId.Count > 0;
        for (var index = _damagePopups.Count - 1; index >= 0; index--)
        {
            _damagePopups[index].Age += delta;
            if (_damagePopups[index].Age >= DamagePopupDuration)
            {
                _damagePopups.RemoveAt(index);
            }
        }

        foreach (var unitId in _hitFlashByUnitId.Keys.ToList())
        {
            var remaining = _hitFlashByUnitId[unitId] - delta;
            if (remaining <= 0f)
            {
                _hitFlashByUnitId.Remove(unitId);
                continue;
            }

            _hitFlashByUnitId[unitId] = remaining;
        }

        foreach (var unitId in _displayHpByUnitId.Keys.ToList())
        {
            var unit = _state.Units.FirstOrDefault(unit => unit.Id == unitId && unit.IsAlive);
            if (unit is null)
            {
                _displayHpByUnitId.Remove(unitId);
                continue;
            }

            var current = _displayHpByUnitId[unitId];
            var target = unit.Hp;
            var step = Math.Max(0.1f, 18f * delta);
            if (MathF.Abs(current - target) <= step)
            {
                _displayHpByUnitId.Remove(unitId);
                continue;
            }

            _displayHpByUnitId[unitId] = current + (Math.Sign(target - current) * step);
        }

        return needsRedraw;
    }

    private string RescueInstructionText()
    {
        if (_state.RelayStation != TileCoord.None || _state.FuelCache != TileCoord.None)
        {
            return "RLY = Relay, FUEL = Fuel Cache. Tech, Engineer, or Rig captures by waiting on the marker twice.";
        }

        return _state.RequiresScoutSurvival && _state.ScoutRescued
            ? "Scout-7 rescued. Use infantry and armor together to defeat every red unit."
            : _state.RescueInstruction;
    }

    private string ForecastMathText(UnitState attacker, UnitState defender)
    {
        var attackerProfile = BattleRules.GetProfile(attacker.Type);
        var defenderProfile = BattleRules.GetProfile(defender.Type);
        var terrainDefense = TerrainDefenseText(_state.GetTerrain(defender.Position));
        return $"Why: {attacker.Type} ATK {attackerProfile.Attack} + HP bonus vs {defender.Type} DEF {defenderProfile.Defense}{terrainDefense}.";
    }

    private static string TeamName(Team team) => team == Team.Player ? "Blue" : "Red";

    private static string TerrainDefenseText(TerrainType terrain) => terrain switch
    {
        TerrainType.Cover => " + cover",
        TerrainType.Hq => " + HQ cover",
        _ => string.Empty
    };

    private static string UnitStatsText(UnitState unit)
    {
        var profile = BattleRules.GetProfile(unit.Type);
        return $"Stats: HP {unit.Hp}/{profile.MaxHp}, ATK {profile.Attack}, DEF {profile.Defense}, MOV {profile.Move}.";
    }

    private static string UnitTypeLabel(UnitType type) => type switch
    {
        UnitType.Infantry => "INF",
        UnitType.Armor => "ARM",
        UnitType.Scout => "SCT",
        UnitType.Engineer => "ENG",
        UnitType.Sapper => "SAP",
        UnitType.Lancer => "LNC",
        UnitType.Striker => "STK",
        UnitType.FieldRig => "RIG",
        UnitType.SiegeBreaker => "SIE",
        _ => "?"
    };

    private int TerrainSpriteIndex(TerrainType terrain, TileCoord coord) => terrain switch
    {
        TerrainType.Plain => PlainVariantIndex(coord),
        TerrainType.Road => RoadVariantIndex(coord),
        TerrainType.Cover => VariantHash(coord) % 2 == 0 ? 2 : 10,
        TerrainType.Hq => 3,
        TerrainType.Ridge => 4,
        TerrainType.Workshop => 11,
        _ => 0
    };

    private static int PlainVariantIndex(TileCoord coord)
    {
        var variants = new[] { 0, 5, 6, 7, 8 };
        return variants[VariantHash(coord) % variants.Length];
    }

    private int RoadVariantIndex(TileCoord coord)
    {
        var hasHorizontalNeighbor = IsTerrain(coord with { X = coord.X - 1 }, TerrainType.Road)
            || IsTerrain(coord with { X = coord.X + 1 }, TerrainType.Road);
        var hasVerticalNeighbor = IsTerrain(coord with { Y = coord.Y - 1 }, TerrainType.Road)
            || IsTerrain(coord with { Y = coord.Y + 1 }, TerrainType.Road);
        return hasHorizontalNeighbor && !hasVerticalNeighbor ? 9 : 1;
    }

    private bool IsTerrain(TileCoord coord, TerrainType terrain) => _state.Contains(coord) && _state.GetTerrain(coord) == terrain;

    private static int VariantHash(TileCoord coord) => Math.Abs((coord.X * 31) ^ (coord.Y * 17));

    private static int UnitSpriteIndex(UnitType type) => type switch
    {
        UnitType.Infantry => 0,
        UnitType.Armor => 1,
        UnitType.Scout => 2,
        UnitType.Engineer => 3,
        UnitType.Sapper => 4,
        UnitType.Lancer => 5,
        UnitType.Striker => 6,
        UnitType.FieldRig => 7,
        UnitType.SiegeBreaker => 8,
        _ => 0
    };

    private static string UnitRoleText(UnitType type) => type switch
    {
        UnitType.Infantry => "Infantry: reliable rescue troop, best for clearing light raiders.",
        UnitType.Armor => "Armor: tough front-line tank, best at holding roads.",
        UnitType.Scout => "Scout: fast wedge vehicle, fragile until rescued.",
        UnitType.Engineer => "Engineer: support unit that captures relay and fuel objectives.",
        UnitType.Sapper => "Sapper: fragile raider that threatens support units.",
        UnitType.Lancer => "Lancer: anti-armor unit that punishes heavy vehicles.",
        UnitType.Striker => "Striker: fast raider or response unit, strong into support units.",
        UnitType.FieldRig => "Field rig: durable support unit that can secure mission objectives.",
        UnitType.SiegeBreaker => "Siege breaker: slow heavy unit built to crack armor and HQ lines.",
        _ => string.Empty
    };

    private static IEnumerable<string> WrapText(string text, int maxChars)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var currentLine = string.Empty;

        foreach (var word in words)
        {
            if (currentLine.Length == 0)
            {
                currentLine = word;
                continue;
            }

            var candidate = $"{currentLine} {word}";
            if (candidate.Length <= maxChars)
            {
                currentLine = candidate;
                continue;
            }

            yield return currentLine;
            currentLine = word;
        }

        if (currentLine.Length > 0)
        {
            yield return currentLine;
        }
    }

    private void TrySelectUnit()
    {
        var unit = BattleRules.GetLivingUnitAt(_state, _cursor);
        if (unit is null || unit.Team != Team.Player || unit.HasActed || BattleRules.IsScoutStranded(_state, unit))
        {
            AddMessage("Select a ready blue unit. Scout-7 is stranded until infantry or armor moves next to them.");
            QueueRedraw();
            return;
        }

        _selectedUnitId = unit.Id;
        _actionMode = unit.HasMoved;
        AddMessage($"Selected {unit.Id}. Choose move, adjacent attack, or wait.");
        QueueRedraw();
    }
}
