Skill Tree In Unity (Tutorial) -- Part I: The User Interface

Dodane pliki:
- Skill_Tree_UI.cs
- Skill_Node_UI.cs

Ten etap robi tylko interfejs drzewka umiejetnosci:
- panel SkillTreePanel
- otwieranie/zamykanie klawiszem K
- zamykanie klawiszem Escape
- opcjonalna pauza gry po otwarciu panelu
- node/przycisk skilla z nazwa, opisem, kosztem i ikonka

Najwazniejsze ustawienie:
Skill_Tree_UI dawaj na aktywny obiekt, np. PlayerUI, a NIE na SkillTreePanel, jesli panel bedzie chowany.

Zalecana struktura:
PlayerUI
- StatsPanel
- healthUI
- ExperienceUI
- SkillTreePanel
  - Background
  - SkillNodesContainer
    - SkillNode_Attack
    - SkillNode_Health
    - SkillNode_Speed
  - CloseButton

Na SkillTreePanel dodaj CanvasGroup.
W Skill_Tree_UI ustaw:
- Skill Tree Panel = SkillTreePanel
- Skill Tree Canvas Group = CanvasGroup ze SkillTreePanel
- Toggle Key = K
- Close Key = Escape
- Pause Game When Panel Open = true

Do SkillNodesContainer dodaj Grid Layout Group, zeby skille same ustawialy sie w siatce.
Na grafice panelu ustaw Image Type = Sliced, jezeli sprite ma ustawione bordery 9-slicing.
