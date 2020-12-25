using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace LibMESone
{
    public class CParent : CChild
    {

        protected Dictionary<ulong, CChild> Children = new Dictionary<ulong, CChild>();


        public void CUD<T>(IDictionary<ulong, IDictionary<string, object>> children) where T : CChild, new()
        {
            try
            {

                IEnumerable<ulong> waste = Children.Keys.Except(children.Keys);
                IEnumerable<ulong> missing = children.Keys.Except(Children.Keys);

                foreach (ulong id in waste)
                {
                    if (Children[id] is IDisposable)
                    {
                        IDisposable instance = (IDisposable)Children[id];
                        instance.Dispose();
                    }

                    Children.Remove(id);
                }

                foreach (ulong id in missing)
                {
                    Children.Add(id, null);
                }

                foreach (var child in children)
                {

                    if (Children[child.Key] == null)
                    {
                        Children[child.Key] = new T();
                    }

                    foreach (var prop in child.Value)
                    {
                        PropertyInfo pi = Children[child.Key].GetType().GetProperty(prop.Key);

                        if (pi != null)
                        {
                            object value = null;

                            try
                            {
                                value = Convert.ChangeType(prop.Value, pi.PropertyType);
                            }
                            catch
                            {

                            }

                            pi.SetValue(Children[child.Key], value, null);

                        }


                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
            }
        }

    }
}
