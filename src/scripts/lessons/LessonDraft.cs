using System;
using System.Collections.Generic;

namespace WizardBattle.Lessons;

// Редактируемый учебный материал. (Де)сериализация через System.Text.Json.
public class LessonDraft
{
    // Слово -> эталонные предложения с ответами между #
    // Например: "to jump" -> ["The quick brown fox #jumps# over the lazy dog."]
    public Dictionary<string, List<string>> SentencesByWord { get; set; }
        = new(StringComparer.Ordinal);

    // Предложение, включая #ответы# -> идентификатор заклинания
    public Dictionary<string, string> SpellIdBySentence { get; set; }
        = new(StringComparer.Ordinal);
}
