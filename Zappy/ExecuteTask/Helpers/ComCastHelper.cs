using html;
using System;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;

namespace Zappy.ExecuteTask.Helpers
{
    internal static class ComCastHelper
    {

        public static ToInterface CastAs<FromInterface, ToInterface>(FromInterface baseObject) where ToInterface : class
        {
            ToInterface local = default(ToInterface);
            try
            {
                local = baseObject as ToInterface;
            }
            catch (SystemException exception)
            {
                CrapyLogger.log.Error(exception);
            }
            return local;
        }

        public static int GetUniqueNumberAfterCast<FromInterface>(FromInterface baseObject) =>
            SafeCast<FromInterface, IHTMLUniqueName>(baseObject).uniqueNumber;

        public static ToInterface SafeCast<FromInterface, ToInterface>(FromInterface baseObject) where ToInterface : class
        {
            ToInterface local = TryCast<FromInterface, ToInterface>(baseObject);
            if (local == null)
            {
                CrapyLogger.log.ErrorFormat("IEDOM : ComCastHelper : SafeCast : Queried object is null");
                throw new ZappyTaskControlNotAvailableException();
            }
            return local;
        }

        public static T SafeCastToHTMLDocument<T>(object baseObject) where T : class =>
            SafeCast<object, T>(baseObject);

        public static ToInterface TryCast<FromInterface, ToInterface>(FromInterface baseObject) where ToInterface : class
        {
            if (baseObject == null)
            {
                CrapyLogger.log.ErrorFormat("IEDOM : ComCastHelper : TryCast : BaseObject is null");
                throw new ZappyTaskControlNotAvailableException();
            }
            ToInterface local = default(ToInterface);
            try
            {
                local = baseObject as ToInterface;
            }
            catch (SystemException exception)
            {
                CrapyLogger.log.Error(exception);
            }
            return local;
        }
    }
}