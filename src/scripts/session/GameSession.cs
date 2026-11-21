using System;
using Godot;
using WizardBattle.Lessons;

namespace WizardBattle.Session;

// Autoload: сохраняет материал при переключении экранов.
public partial class GameSession : Node
{
    public LessonDraft Draft { get; private set; }

    public SessionLesson Lesson { get; private set; }

    public void BeginPreparation()
    {
        Draft = new LessonDraft();
        Lesson = null;
    }

    public void StartMatch()
    {
        if (Draft is null)
            throw new InvalidOperationException("Сначала начните подготовку учебного материала.");

        Lesson = new SessionLesson(Draft);
    }

    public void EndSession()
    {
        Draft = null;
        Lesson = null;
    }
}
