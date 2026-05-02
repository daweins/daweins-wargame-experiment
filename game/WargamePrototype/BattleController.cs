using Godot;
using Wargame.Core;

[GlobalClass]
public partial class BattleController : Control
{
    private const int BoardOriginX = 32;
    private const int BoardOriginY = 32;
    private const int BoardPixelHeight = 8 * TileSize;
    private const int BoardPixelWidth = 12 * TileSize;
    private const int PanelContentWidth = 360;
    private const int PanelX = 840;
    private const int SpriteSize = 64;
    private const int TileSize = 64;

    private readonly Color _cover = new("#356c54");
    private readonly Color _enemy = new("#e45d47");
    private readonly Color _enemyDark = new("#7d2431");
    private readonly Color _grid = new("#1b2735aa");
    private readonly Color _hq = new("#7457c7");
    private readonly Color _panel = new("#162130");
    private readonly Color _plain = new("#6c9e5f");
    private readonly Color _player = new("#4aa4e8");
    private readonly Color _playerDark = new("#1f5b9d");
    private readonly Color _road = new("#b6945c");
    private readonly Color _text = new("#f0f2d4");
    private readonly Color _warning = new("#f6c85f");

    private readonly List<string> _messages = [];
    private BattleState _state = FirstMissionFactory.Create();
    private BattleState? _pendingMoveUndoState;
    private TileCoord _cursor = new(2, 3);
    private TileCoord _pendingMoveOriginalCursor;
    private string? _pendingMoveUnitId;
    private string? _selectedUnitId;
    private bool _actionMode;
    private Texture2D? _terrainSprites;
    private Texture2D? _unitSprites;

    private readonly record struct UnitSnapshot(string Id, Team Team, UnitType Type, TileCoord Position, int Hp, bool IsAlive);

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(1280, 800);
        _terrainSprites = LoadSpriteSheet("res://assets/sprites/terrain.png");
        _unitSprites = LoadSpriteSheet("res://assets/sprites/units.png");
        AddMessage("Blue units are yours. Red units are raiders. Yellow ring marks Scout-7.");
        AddMessage("Rescue rule: move any infantry or armor next to Scout-7, then keep them alive.");
        AddMessage("Briefing: hold the HQ, rescue Scout-7, and clear the wider raider patrol.");
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawBackdrop();
        DrawBoard();
        DrawHighlights();
        DrawUnits();
        DrawCursor();
        DrawPanel();
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (inputEvent.IsEcho())
        {
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
            if (keyEvent.Keycode == Key.R) ResetMission();
            if (keyEvent.Keycode == Key.Tab) CycleReadyUnit();
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
        var wasScoutRescued = _state.ScoutRescued;
        var result = BattleRules.ApplyCommand(_state, command);
        AddMessage(result.Message);
        if (!wasScoutRescued && _state.ScoutRescued)
        {
            AddMessage("Scout-7 secured. They can act on later turns if they survive.");
        }

        if (_state.Outcome == BattleOutcome.PlayerVictory)
        {
            AddMessage("Mission complete. Debrief: improvised defense, measurable panic, acceptable science.");
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

        for (var row = 0; row < _state.Height; row++)
        {
            for (var column = 0; column < _state.Width; column++)
            {
                var coord = new TileCoord(column, row);
                var rect = TileRect(coord);
                DrawRect(rect, TileColor(_state.GetTerrain(coord)));
                DrawTerrainSprite(_state.GetTerrain(coord), rect);
                DrawRect(rect, _grid, filled: false, width: 1);
            }
        }

        DrawRect(boardRect.Grow(2), new Color("#f2d17a"), filled: false, width: 3);
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
                var rect = TileRect(coord).Grow(-11);
                DrawRect(rect, new Color("#62d7ff40"));
                DrawRect(rect, new Color("#9ff0ff9a"), filled: false, width: 2);
            }
        }

        foreach (var coord in BattleRules.GetAttackableCoords(_state, selectedUnit))
        {
            var rect = TileRect(coord).Grow(-9);
            DrawRect(rect, new Color("#ff655551"));
            DrawRect(rect, new Color("#ffd1bd"), filled: false, width: 2);
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
        DrawString(font, new Vector2(PanelX + 24, yPosition), "OPERATION: FIELD PEER REVIEW", HorizontalAlignment.Left, -1, 18, _text);
        yPosition += 28;

        DrawModeBanner(font, ref yPosition);
        yPosition += 10;

        DrawSectionHeader(font, "OBJECTIVE", ref yPosition);
        yPosition = DrawWrappedLine(font, $"Turn {_state.Turn}: {ObjectiveText()}", yPosition, 16, _text, 36, 2);
        yPosition = DrawWrappedLine(font, RescueInstructionText(), yPosition + 4, 15, _warning, 34, 3);
        yPosition = DrawWrappedLine(font, ModeInstructionText(), yPosition + 8, 15, _text, 36, 3);
        yPosition = DrawWrappedLine(font, "E/Start: end turn. Tab/RB: cycle ready units. Esc/B: cancel.", yPosition + 4, 14, _text, 38, 3);
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

    private void DrawFallbackTerrainPattern(TerrainType terrain, Rect2 rect)
    {
        switch (terrain)
        {
            case TerrainType.Plain:
                DrawRect(new Rect2(rect.Position + new Vector2(10, 12), new Vector2(12, 4)), new Color("#83b96c"));
                DrawRect(new Rect2(rect.Position + new Vector2(34, 22), new Vector2(18, 5)), new Color("#527f4f"));
                DrawRect(new Rect2(rect.Position + new Vector2(19, 42), new Vector2(11, 4)), new Color("#7db467"));
                break;
            case TerrainType.Road:
                DrawRect(new Rect2(rect.Position + new Vector2(0, 17), new Vector2(TileSize, 30)), new Color("#d2b16e"));
                DrawRect(new Rect2(rect.Position + new Vector2(0, 17), new Vector2(TileSize, 4)), new Color("#f0ce84"));
                DrawRect(new Rect2(rect.Position + new Vector2(0, 43), new Vector2(TileSize, 4)), new Color("#7d6845"));
                DrawRect(new Rect2(rect.Position + new Vector2(12, 30), new Vector2(12, 4)), new Color("#8a7147"));
                DrawRect(new Rect2(rect.Position + new Vector2(40, 29), new Vector2(10, 4)), new Color("#f4d78f"));
                break;
            case TerrainType.Cover:
                DrawRect(new Rect2(rect.Position + new Vector2(8, 29), new Vector2(48, 18)), new Color("#244638"));
                DrawRect(new Rect2(rect.Position + new Vector2(11, 15), new Vector2(16, 17)), new Color("#5aa06b"));
                DrawRect(new Rect2(rect.Position + new Vector2(18, 8), new Vector2(12, 13)), new Color("#7fc279"));
                DrawRect(new Rect2(rect.Position + new Vector2(34, 18), new Vector2(16, 18)), new Color("#437c5b"));
                DrawRect(new Rect2(rect.Position + new Vector2(39, 10), new Vector2(12, 12)), new Color("#6aae70"));
                DrawRect(new Rect2(rect.Position + new Vector2(21, 32), new Vector2(5, 17)), new Color("#4a3227"));
                DrawRect(new Rect2(rect.Position + new Vector2(42, 34), new Vector2(5, 16)), new Color("#4a3227"));
                break;
            case TerrainType.Hq:
                DrawRect(new Rect2(rect.Position + new Vector2(10, 42), new Vector2(44, 10)), new Color("#3c2e67"));
                DrawRect(new Rect2(rect.Position + new Vector2(16, 20), new Vector2(32, 28)), new Color("#c9b9ff"));
                DrawRect(new Rect2(rect.Position + new Vector2(20, 16), new Vector2(24, 8)), _warning);
                DrawRect(new Rect2(rect.Position + new Vector2(24, 30), new Vector2(7, 10)), new Color("#5e45a6"));
                DrawRect(new Rect2(rect.Position + new Vector2(36, 30), new Vector2(7, 10)), new Color("#5e45a6"));
                DrawRect(new Rect2(rect.Position + new Vector2(18, 48), new Vector2(30, 4)), new Color("#f0e7ff"));
                break;
            case TerrainType.Ridge:
                DrawRect(new Rect2(rect.Position + new Vector2(7, 34), new Vector2(50, 14)), new Color("#111926"));
                DrawRect(new Rect2(rect.Position + new Vector2(11, 26), new Vector2(18, 15)), new Color("#38455a"));
                DrawRect(new Rect2(rect.Position + new Vector2(25, 18), new Vector2(19, 23)), new Color("#4c5a70"));
                DrawRect(new Rect2(rect.Position + new Vector2(40, 29), new Vector2(14, 12)), new Color("#2b3547"));
                DrawRect(new Rect2(rect.Position + new Vector2(26, 19), new Vector2(10, 4)), new Color("#8a96aa"));
                break;
        }
    }

    private void DrawTerrainSprite(TerrainType terrain, Rect2 destination)
    {
        if (_terrainSprites is null)
        {
            DrawFallbackTerrainPattern(terrain, destination);
            return;
        }

        var source = new Rect2(TerrainSpriteIndex(terrain) * SpriteSize, 0, SpriteSize, SpriteSize);
        DrawTextureRectRegion(_terrainSprites, destination, source);
    }

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
        var font = GetThemeDefaultFont();
        foreach (var unit in _state.Units.Where(unit => unit.IsAlive))
        {
            var rect = TileRect(unit.Position).Grow(-5);
            var teamColor = unit.Team == Team.Player ? _player : _enemy;
            var baseRect = new Rect2(rect.Position + new Vector2(5, rect.Size.Y - 16), new Vector2(rect.Size.X - 10, 12));
            DrawRect(new Rect2(rect.Position + new Vector2(4, 7), new Vector2(rect.Size.X - 8, rect.Size.Y - 4)), new Color("#0509107f"));
            DrawRect(new Rect2(baseRect.Position + new Vector2(4, 3), baseRect.Size), new Color("#0509109f"));
            DrawRect(baseRect, new Color("#101820"));
            DrawRect(baseRect.Grow(-2), teamColor);
            DrawRect(new Rect2(baseRect.Position + new Vector2(6, 2), new Vector2(baseRect.Size.X - 12, 3)), new Color("#ffffff45"));
            DrawUnitSprite(unit, rect);
            DrawTeamBadge(unit.Team, rect);

            if (BattleRules.IsScoutStranded(_state, unit))
            {
                DrawRect(rect.Grow(2), new Color("#050910"), filled: false, width: 6);
                DrawRect(rect.Grow(1), _warning, filled: false, width: 3);
                DrawRect(new Rect2(rect.Position + new Vector2(16, -7), new Vector2(25, 7)), _warning);
            }

            var profile = BattleRules.GetProfile(unit.Type);
            var hpWidth = Math.Max(4, (int)(46 * (unit.Hp / (float)profile.MaxHp)));
            DrawRect(new Rect2(rect.Position + new Vector2(3, rect.Size.Y - 8), new Vector2(46, 6)), new Color("#250f14"));
            DrawRect(new Rect2(rect.Position + new Vector2(3, rect.Size.Y - 8), new Vector2(hpWidth, 6)), HpColor(unit, profile));
            DrawUnitLabels(font, unit, rect);
        }
    }

    private void EndTurn()
    {
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
        var scout = _state.Units.FirstOrDefault(unit => unit.Id == _state.ScoutId);
        if (scout is not { IsAlive: true })
        {
            return "Defeat reason: Scout-7 was destroyed before extraction.";
        }

        if (_state.Units.Any(unit => unit.IsAlive && unit.Team == Team.Enemy && unit.Position == _state.PlayerHq))
        {
            return "Defeat reason: a red unit reached the purple HQ tile.";
        }

        return "Defeat reason: mission objective failed.";
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
        DrawString(font, new Vector2(PanelX + 46, yPosition), "Blue = your units", HorizontalAlignment.Left, 145, 14, _text);
        DrawLegendSwatch(new Vector2(PanelX + 194, yPosition - 12), _enemy, _enemyDark);
        DrawString(font, new Vector2(PanelX + 216, yPosition), "Red = raiders", HorizontalAlignment.Left, 125, 14, _text);
        yPosition += 22;
        DrawRect(new Rect2(PanelX + 24, yPosition - 12, 16, 10), _warning);
        DrawRect(new Rect2(PanelX + 27, yPosition - 9, 10, 4), new Color("#fff0a0"));
        DrawString(font, new Vector2(PanelX + 46, yPosition), "Yellow ring = stranded Scout-7", HorizontalAlignment.Left, 290, 14, _text);
        yPosition += 22;
        yPosition = DrawWrappedLine(font, "Shapes: soldier = infantry, tank = armor, wedge = scout", yPosition, 14, _text, 41, 2);
    }

    private void DrawLegendSwatch(Vector2 position, Color light, Color dark)
    {
        DrawRect(new Rect2(position, new Vector2(16, 14)), dark);
        DrawRect(new Rect2(position + new Vector2(3, 3), new Vector2(10, 8)), light);
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

    private void DrawTeamBadge(Team team, Rect2 rect)
    {
        var badgeColor = team == Team.Player ? _playerDark : _enemyDark;
        var badgePosition = team == Team.Player
            ? rect.Position + new Vector2(3, 3)
            : rect.Position + new Vector2(rect.Size.X - 13, 3);
        DrawRect(new Rect2(badgePosition, new Vector2(10, 10)), badgeColor);
        DrawRect(new Rect2(badgePosition + new Vector2(3, 3), new Vector2(4, 4)), _text);
    }

    private void DrawUnitLabels(Font font, UnitState unit, Rect2 rect)
    {
        var profile = BattleRules.GetProfile(unit.Type);
        var labelColor = unit.Hp <= profile.MaxHp / 2 ? _warning : _text;
        DrawRect(new Rect2(rect.Position + new Vector2(13, -6), new Vector2(30, 14)), new Color("#071018dd"));
        DrawString(font, rect.Position + new Vector2(17, 6), UnitTypeLabel(unit.Type), HorizontalAlignment.Left, 24, 10, _text);
        DrawRect(new Rect2(rect.Position + new Vector2(12, rect.Size.Y - 22), new Vector2(31, 15)), new Color("#071018dd"));
        DrawString(font, rect.Position + new Vector2(17, rect.Size.Y - 10), $"{unit.Hp}", HorizontalAlignment.Left, 24, 12, labelColor);
    }

    private void DrawUnitSprite(UnitState unit, Rect2 rect)
    {
        if (_unitSprites is null)
        {
            DrawFallbackUnitSprite(unit, rect);
            return;
        }

        var source = new Rect2(UnitSpriteIndex(unit.Type) * SpriteSize, (unit.Team == Team.Player ? 0 : 1) * SpriteSize, SpriteSize, SpriteSize);
        DrawTextureRectRegion(_unitSprites, rect, source);
    }

    private void MoveCursor(TileCoord coord)
    {
        if (!_state.Contains(coord))
        {
            return;
        }

        _cursor = coord;
        QueueRedraw();
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

    private string ObjectiveText()
    {
        if (_state.Outcome == BattleOutcome.PlayerVictory) return "Mission complete";
        if (_state.Outcome == BattleOutcome.PlayerDefeat) return "Mission failed";
        if (!_state.ScoutRescued) return "Hold HQ, reach Scout-7";
        return "Scout secure, defeat raiders";
    }

    private string ModeInstructionText()
    {
        if (_state.IsComplete)
        {
            return "Mission ended. Press Enter/A or R to restart.";
        }

        if (_selectedUnitId is null)
        {
            return "Select mode: move cursor to a blue ready unit, then press Enter/A.";
        }

        if (!_actionMode)
        {
            return "Move mode: blue squares are legal moves. Press Enter/A on one to move.";
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
        _state = FirstMissionFactory.Create();
        _cursor = new TileCoord(2, 3);
        _selectedUnitId = null;
        _actionMode = false;
        _pendingMoveUndoState = null;
        _pendingMoveUnitId = null;
        _messages.Clear();
        AddMessage("Rescue rule: move any infantry or armor next to Scout-7, then keep them alive.");
        AddMessage("Briefing: hold the HQ, rescue Scout-7, and clear the wider raider patrol.");
        AddMessage("Reset complete. The safety committee has chosen denial.");
        QueueRedraw();
    }

    private Color TileColor(TerrainType terrain) => terrain switch
    {
        TerrainType.Plain => _plain,
        TerrainType.Road => _road,
        TerrainType.Cover => _cover,
        TerrainType.Hq => _hq,
        TerrainType.Ridge => new Color("#202938"),
        _ => _plain
    };

    private Rect2 TileRect(TileCoord coord) => new(
        BoardOriginX + coord.X * TileSize,
        BoardOriginY + coord.Y * TileSize,
        TileSize,
        TileSize);

    private string RescueInstructionText() => _state.ScoutRescued
        ? "Scout-7 rescued. Use infantry and armor together to defeat every red unit."
        : "Rescue Scout-7: move infantry or armor to a tile directly next to them.";

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
        _ => "?"
    };

    private static int TerrainSpriteIndex(TerrainType terrain) => terrain switch
    {
        TerrainType.Plain => 0,
        TerrainType.Road => 1,
        TerrainType.Cover => 2,
        TerrainType.Hq => 3,
        TerrainType.Ridge => 4,
        _ => 0
    };

    private static int UnitSpriteIndex(UnitType type) => type switch
    {
        UnitType.Infantry => 0,
        UnitType.Armor => 1,
        UnitType.Scout => 2,
        _ => 0
    };

    private static string UnitRoleText(UnitType type) => type switch
    {
        UnitType.Infantry => "Infantry: reliable rescue troop, best for clearing light raiders.",
        UnitType.Armor => "Armor: tough front-line tank, best at holding roads.",
        UnitType.Scout => "Scout: fast wedge vehicle, fragile until rescued.",
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
