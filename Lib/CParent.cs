using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Lib
{
    public class CParent : CChild
    {

        protected Dictionary<ulong, CChild> Children = new Dictionary<ulong, CChild>();


        public void CUD<T>(Dictionary<ulong, Dictionary<string, object>> children) where T : CChild, new()
        {
            try
            {
                if (children != null)
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
                                //object value = null;

                                //try
                                //{
                                // value = Convert.ChangeType(prop.Value, pi.PropertyType);
                                //}
                                //catch(Exception ex)
                                //{
                                //Logger.Warn(ex, $"Can't convert value {pi.Name}");
                                //}

                                //pi.SetValue(Children[child.Key], value, null);
                                pi.SetValue(Children[child.Key], prop.Value, null);

                            }


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
