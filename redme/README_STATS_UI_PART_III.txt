Stats System Part III - Connecting the UI and Pausing

Nowe / zmienione pliki:
- Game_Pause_Manager.cs
- Player_Stats_UI.cs
- Player_Attack.cs
- playermovment.cs
- Enemy_Movment.cs

Najważniejsze ustawienia w Unity:
1. Player_Stats_UI musi być na aktywnym obiekcie, np. PlayerUI, nie na StatsPanel.
2. W Player_Stats_UI podepnij StatsPanel, teksty, Stats gracza i player_helf gracza.
3. Zaznacz Pause Game When Panel Open.
4. StatsPanel może być aktywny w edytorze; skrypt schowa go przy starcie, jeśli Show On Start jest odznaczone.
5. Klawisz C otwiera/zamyka panel statystyk.
6. Escape zamyka panel.
7. Kiedy panel jest otwarty, Time.timeScale = 0 i gra jest zatrzymana.

Do przycisków UI możesz podpiąć:
- Player_Stats_UI.ShowStatsPanel()
- Player_Stats_UI.HideStatsPanel()
- Player_Stats_UI.ToggleStatsPanel()
