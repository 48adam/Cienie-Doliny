EXPERIENCE AND LEVELLING SYSTEM

Dodane pliki:
- Player_Experience.cs
- Player_Experience_UI.cs
- Test_Give_Experience.cs

Zmieniony plik:
- Enemy_Health.cs

Co zrobić w Unity:
1. Dodaj Player_Experience na gracza.
2. Ustaw Current Level = 1, Current Experience = 0, Experience To Next Level = 100.
3. Na wrogach w Enemy_Health ustaw Experience Reward, np. 25.
4. W Canvas/PlayerUI utwórz pasek UI > Slider i nazwij go ExperienceSlider.
5. Dodaj tekst TMP LevelText oraz opcjonalny ExperienceText.
6. Na PlayerUI dodaj Player_Experience_UI.
7. Podepnij Player Experience, Experience Slider, Level Text i Experience Text.
8. Po zabiciu przeciwnika pasek XP powinien się zwiększyć.

Opcjonalny test:
- Dodaj Test_Give_Experience na gracza.
- W Play Mode kliknij X, żeby dodać testowe XP bez zabijania przeciwnika.
