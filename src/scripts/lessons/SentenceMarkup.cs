using System;

namespace WizardBattle.Lessons;

public static class SentenceMarkup
{
    // Проверяет эталон преподавателя.
    public static bool TryValidate(string sentence, out string error)
    {
        if (string.IsNullOrWhiteSpace(sentence))
        {
            error = "Введите предложение.";
            return false;
        }

        bool insideAnswer = false;
        bool answerHasText = false;
        bool hasCompletedPair = false;

        foreach (char character in sentence)
        {
            if (character != '#')
            {
                if (insideAnswer && !char.IsWhiteSpace(character))
                    answerHasText = true;

                continue;
            }

            if (!insideAnswer)
            {
                insideAnswer = true;
                answerHasText = false;
                continue;
            }

            if (!answerHasText)
            {
                error = "Между парой # должен быть ответ. Пустые ## и ответы из пробелов недопустимы.";
                return false;
            }

            insideAnswer = false;
            hasCompletedPair = true;
        }

        if (insideAnswer)
        {
            error = "Не закрыт тег #: добавьте вторую решётку после ответа.";
            return false;
        }

        if (!hasCompletedPair)
        {
            error = "Выделите хотя бы один ответ парой решёток: #ответ#.";
            return false;
        }

        error = null;
        return true;
    }
}
