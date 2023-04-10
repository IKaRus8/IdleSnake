using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Extensions.Core
{
    public static class NullableExtensions
    {
        [ContractAnnotation("nullable:null => halt")]
        public static void CheckForNull<T>(
            [CanBeNull] this T nullable,
            [CallerMemberName] string objName = "")
            where T : class
        {
            if (nullable == null)
            {
                throw CreateNullException(objName);
            }
        }

        [ContractAnnotation("collection:null => halt")]
        public static void CheckSelfAndItemsForNullOrEmpty<T>(
            [CanBeNull] this T collection,
            [CallerMemberName] string objName = "")
            where T : System.Collections.ICollection
        {
            if (collection == null)
            {
                throw CreateNullException(objName);
            }

            if (collection.Count == 0)
            {
                throw CreateNullException($"{objName} empty");
            }

            foreach (var item in collection)
            {
                if (item == null)
                {
                    throw CreateNullException($"{objName} items");
                }
            }
        }
        
        private static Exception CreateNullException(string memberName)
        {
            return new InvalidOperationException($"Expected nullable (in {memberName} method) to have value");
        }
    }
}