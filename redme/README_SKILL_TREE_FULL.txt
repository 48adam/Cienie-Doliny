SKILL TREE FULL IMPLEMENTATION

1. Na graczu musi być Player_Experience oraz Stats.
2. Na obiekcie UI, który jest zawsze aktywny, dodaj Skill_Tree_UI.
3. Na skillPanel dodaj CanvasGroup.
4. Na obiekcie skillPanel albo na dziecięcym kontenerze dodaj Skill_Tree_Manager.
5. W Skill_Tree_Manager podepnij:
   - Player Experience = Player_Experience z gracza
   - Player Stats = Stats z gracza
   - Skill Points Text = tekst z napisem skill points
   - Skill Nodes = wszystkie przyciski skilli z komponentem Skill_Node_UI
6. Na każdym przycisku skilla dodaj Skill_Node_UI.
7. W Button > OnClick podepnij ten sam przycisk i wybierz Skill_Node_UI.OnSkillClicked().
8. W Skill_Node_UI ustaw Effect Type:
   - MaxHealth
   - Damage
   - MoveSpeed
   - Armor
9. Effect Amount oznacza ile statystyki daje jeden punkt.
10. Cost oznacza ile punktów skilla kosztuje jeden zakup.
11. Max Rank oznacza ile razy można ulepszyć skill.
12. Prerequisite Skills to lista skilli, które muszą być kupione wcześniej.
13. Player_Experience daje skillPointsPerLevel punktów skilla przy level upie.
