using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace WizardBattle.Lessons;

public static class LessonDraftFile
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public static string Serialize(LessonDraft draft)
    {
        Validate(draft);
        return JsonSerializer.Serialize(draft, Options);
    }

    public static LessonDraft Deserialize(string json)
    {
        var draft = JsonSerializer.Deserialize<LessonDraft>(json, Options);
        Validate(draft);
        return draft;
    }

    public static LessonDraft Load(string path) => Deserialize(File.ReadAllText(path));

    public static void Save(string path, LessonDraft draft)
    {
        var json = Serialize(draft);
        // Сначала записываем рядом временный файл, чтобы ошибка записи не испортила старый.
        var temporaryPath = Path.GetFullPath(path) + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    private static void Validate(LessonDraft draft)
    {
        if (draft?.SentencesByWord is null)
            throw new InvalidDataException("Файл должен содержать словарь SentencesByWord.");

        var sentences = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entry in draft.SentencesByWord)
        {
            if (string.IsNullOrWhiteSpace(entry.Key) || entry.Key != entry.Key.Trim())
                throw new InvalidDataException("Слово не должно быть пустым или содержать пробелы по краям.");
            if (entry.Value is null)
                throw new InvalidDataException($"У слова \"{entry.Key}\" отсутствует список предложений.");

            foreach (var sentence in entry.Value)
            {
                if (!SentenceMarkup.TryValidate(sentence, out var error))
                    throw new InvalidDataException($"Слово \"{entry.Key}\": {error}");
                if (sentence != sentence.Trim())
                    throw new InvalidDataException("Предложение содержит пробелы по краям.");
                if (!sentences.Add(sentence))
                    throw new InvalidDataException($"Предложение повторяется: {sentence}");
            }
        }
    }
}
