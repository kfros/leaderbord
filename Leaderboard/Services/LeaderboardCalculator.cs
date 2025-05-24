using Leaderboard.Models;

// ReSharper disable once InvalidXmlDocComment
/**
 * ТРЕБОВАНИЕ:
 *
 * Необходимо реализовать функцию CalculatePlaces.
 * Функция распределяет места пользователей, учитывая ограничения для получения первых мест и набранные пользователями очки.
 * Подробное ТЗ смотреть в readme.txt
 */

/**
 * ТЕХНИЧЕСКИЕ ОГРАНИЧЕНИЯ:
 *
 * количество очков это всегда целое положительное число
 * FirstPlaceMinScore > SecondPlaceMinScore > ThirdPlaceMinScore > 0
 * в конкурсе участвует от 1 до 100 пользователей
 * 2 пользователя не могут набрать одинаковое количество баллов (разные баллы у пользователей гарантируются бизнес-логикой, не стоит усложнять алгоритм)
 * нет ограничений на скорость работы функции и потребляемую ей память
 * при реализации функции разрешается использование любых библиотек, любого стиля написания кода
 * в функцию передаются только валидные данные, которые соответствуют предыдущим ограничениям (проверять это в функции не нужно)
 */

/**
 * ВХОДНЫЕ ДАННЫЕ:
 *
 * usersWithScores - это список пользователей и заработанные каждым из них очки,
 * это неотсортированный массив вида [{userId: "id1", score: score1}, ... , {userId: "idn", score: scoreN}], где score1 ... scoreN различные положительные целые числа, id1 ... idN произвольные неповторяющиеся идентификаторы
 *
 * leaderboardMinScores - это значения минимального количества очков для первых 3 мест
 * это объект вида { FirstPlaceMinScore: score1, SecondPlaceMinScore: score2, ThirdPlaceMinScore : score3 }, где score1 > score2 > score3 > 0 целые положительные числа
 */

/**
 * РЕЗУЛЬТАТ:
 *
 * Функция должна вернуть пользователей с занятыми ими местами
 * Массив вида (сортировка массива не важна): [{UserId: "id1", Place: user1Place}, ..., {UserId: "idN", Place: userNPlace}], где user1Place ... userNPlace это целые положительные числа равные занятым пользователями местами, id1 ... idN идентификаторы пользователей из массива users
 */

namespace Leaderboard.Services;

public class LeaderboardCalculator : ILeaderboardCalculator
{
    public IReadOnlyList<UserWithPlace> CalculatePlaces(IReadOnlyList<IUserWithScore> usersWithScores, LeaderboardMinScores leaderboardMinScores)
    {
        if (usersWithScores == null || usersWithScores.Count == 0)
            return new List<UserWithPlace>();

        // Сортируем пользователей по убыванию очков
        var sortedUsers = usersWithScores
            .OrderByDescending(u => u.Score)
            .ToList();

        var result = new List<UserWithPlace>();
        bool firstPlaceTaken = false;
        bool secondPlaceTaken = false;
        bool thirdPlaceTaken = false;
        int nextAvailablePlace = 4; // Начинаем с 4, т.е. для пользователей, которые не входят в топ-3

        foreach (var user in sortedUsers)
        {
            int assignedPlace;

            // Пробуем назначить место в зависимости от набранных очков и минимальных требований для первых трех мест
            if (!firstPlaceTaken && user.Score >= leaderboardMinScores.FirstPlaceMinScore)
            {
                assignedPlace = 1;
                firstPlaceTaken = true;
            }
            else if (!secondPlaceTaken && user.Score >= leaderboardMinScores.SecondPlaceMinScore)
            {
                assignedPlace = 2;
                secondPlaceTaken = true;
            }
            else if (!thirdPlaceTaken && user.Score >= leaderboardMinScores.ThirdPlaceMinScore)
            {
                assignedPlace = 3;
                thirdPlaceTaken = true;
            }
            else
            {
                // Пользователи, не попавшие в топ-3, получают места начиная с 4
                assignedPlace = nextAvailablePlace;
                nextAvailablePlace++;
            }

            result.Add(new UserWithPlace(user.UserId, assignedPlace));
        }
        return result;
    }
}
