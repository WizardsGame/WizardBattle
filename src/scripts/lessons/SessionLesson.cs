using System;
using System.Collections.Immutable;

namespace WizardBattle.Lessons;

// Неизменяемый снимок материала для запущенной игровой сессии на хосте.
public class SessionLesson
{
    public ImmutableDictionary<string, ImmutableArray<string>> SentencesByWord { get; }

    public ImmutableDictionary<string, string> SpellIdBySentence { get; }

    public SessionLesson(LessonDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(draft.SentencesByWord);
        ArgumentNullException.ThrowIfNull(draft.SpellIdBySentence);

        // Копируем и словарь, и вложенные списки: последующее редактирование
        // черновика не должно менять материал уже запущенной сессии.
        SentencesByWord = draft.SentencesByWord.ToImmutableDictionary(
            entry => entry.Key,
            entry => entry.Value.ToImmutableArray(),
            StringComparer.Ordinal);

        SpellIdBySentence = draft.SpellIdBySentence.ToImmutableDictionary(
            StringComparer.Ordinal);
    }
}
