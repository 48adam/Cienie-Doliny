Creating A Stats System in Unity - Part II: Creating the UI

Dodane pliki:
- Player_Stats_UI.cs
- Stat_Upgrade_Button.cs

Zmienione pliki:
- Stats.cs: event OnStatsChanged + metody AddModifier/SetBaseValue/ClearModifiers.
- player_helf.cs: event OnHealthChanged + odświeżanie HP po zmianie MaxHealth.

Szybka konfiguracja w Unity:
1. Canvas > Panel, nazwij go StatsPanel.
2. W panelu dodaj teksty TMP:
   - HealthText
   - MaxHealthText
   - DamageText
   - MoveSpeedText
   - ArmorText
3. Na Canvas dodaj Player_Stats_UI.
4. Przeciągnij StatsPanel do pola Stats Panel.
5. Przeciągnij komponent Stats gracza do Player Stats.
6. Przeciągnij player_helf gracza do Player Health.
7. Przeciągnij teksty TMP do odpowiednich pól.
8. W Play Mode naciśnij C, żeby otworzyć/zamknąć panel statystyk.

Opcjonalne przyciski upgrade:
1. Dodaj Button do StatsPanel.
2. Na Button dodaj Stat_Upgrade_Button.
3. Ustaw Stat To Upgrade, np. Damage.
4. Ustaw Upgrade Amount, np. 1.
5. W Button > OnClick dodaj obiekt z tym skryptem i wybierz Stat_Upgrade_Button.UpgradeStat().
