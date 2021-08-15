// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using MongoDB.Driver;
//
// namespace BlackDragonAIAPI.StorageHandlers
// {
//     public static class MongoExtensions
//     {
//         public static async Task<IQueryable<T>> AsQueryableAsync<T>(this IAsyncCursor<T> cursor)
//         {
//             IEnumerable<T> resultList = new T[0];
//             do
//             {
//                 resultList = resultList.Concat(cursor.Current);
//
//             } while (await cursor.MoveNextAsync());
//             return resultList.AsQueryable();
//         }
//
//         public static IQueryable<T> AsQueryable<T>(this IAsyncCursor<T> cursor)
//         {
//             IEnumerable<T> resultList = new T[0];
//             do
//             {
//                 resultList = resultList.Concat(cursor.Current);
//
//             } while (cursor.MoveNext());
//             return resultList.AsQueryable();
//         }
//     }
// }
