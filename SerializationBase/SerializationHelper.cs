using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SerializationBase
{
    public class SerializerContext
    {
        public string Path { get; set; }
        public SerializerContext()
        {

        }

        public static void Save<TEntity>(TEntity[] entities, string path) 
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                IFormatter fm = new BinaryFormatter();

                fm.Serialize(fs, entities);
                fs.Close();
            }
        }

        public static TEntity[] Load<TEntity>(string path) where TEntity : new()
        {
            TEntity[] entities = null;

            if (File.Exists(path) == true)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    IFormatter fm = new BinaryFormatter();

                    entities = fm.Deserialize(fs) as TEntity[];
                    fs.Close();
                }
            }
            return entities;
        }

        public static IEnumerable<T> Traverse<T>(IEnumerable<T> roots, Func<T, IEnumerable<T>> children)
        {
            return from root in roots
                   from item in Traverse(root, children)
                   select item;
        }
        public static IEnumerable<T> Traverse<T>(T root, Func<T, IEnumerable<T>> children)
        {
            var stack = new Stack<T>();
            stack.Push(root);
            while (stack.Count != 0)
            {
                T item = stack.Pop();
                yield return item;
                foreach (var child in children(item))
                    stack.Push(child);
            }
        }
    }
}
