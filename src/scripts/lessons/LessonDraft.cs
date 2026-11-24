using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WizardBattle.Lessons;

// Редактируемый учебный материал. (Де)сериализация через System.Text.Json.
public class LessonDraft
{
    // Слово -> эталонные предложения с ответами между #
    // Например: "to jump" -> ["The quick brown fox #jumps# over the lazy dog."]
    [JsonRequired]
    public Dictionary<string, List<string>> SentencesByWord { get; set; }
        = new(StringComparer.Ordinal);

    // Предложение, включая #ответы# -> идентификатор заклинания
    // Назначается на следующем экране и не участвует в импорте/экспорте слов.
    [JsonIgnore]
    public Dictionary<string, string> SpellIdBySentence { get; set; }
        = new(StringComparer.Ordinal);
}
