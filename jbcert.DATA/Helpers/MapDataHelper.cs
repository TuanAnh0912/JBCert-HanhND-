using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace jbcert.DATA.Helpers
{
    public static class MapDataHelper<T> where T : new()
    {
        public static List<T> MapList(DbDataReader dataReader)
        {
            try
            {
                Type businessEntityType = typeof(T);
                List<T> entitys = new List<T>();
                Hashtable hashtable = new Hashtable();
                PropertyInfo[] properties = businessEntityType.GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    hashtable[info.Name.ToUpper()] = info;
                }

                while (dataReader.Read())
                {
                    T newObject = new T();
                    for (int index = 0; index < dataReader.FieldCount; index++)
                    {
                        PropertyInfo info = (PropertyInfo)
                                            hashtable[dataReader.GetName(index).ToUpper()];
                        if ((info != null) && info.CanWrite)
                        {
                            if (string.IsNullOrEmpty(dataReader.GetValue(index).ToString())) continue;
                            var value = dataReader.GetValue(index);
                            info.SetValue(newObject, dataReader.GetValue(index), null);
                        }
                    }
                    entitys.Add(newObject);
                }
                dataReader.Close();
                return entitys;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                dataReader.Close();
            }
        }

        public static List<string> MapListString(DbDataReader dataReader)
        {
            try
            {
                Type businessEntityType = typeof(T);
                List<string> entitys = new List<string>();
                Hashtable hashtable = new Hashtable();
                PropertyInfo[] properties = businessEntityType.GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    hashtable[info.Name.ToUpper()] = info;
                }

                while (dataReader.Read())
                {
                    string newObject = "";
                    for (int index = 0; index < dataReader.FieldCount; index++)
                    {
                        if (string.IsNullOrEmpty(dataReader.GetValue(index).ToString())) continue;
                        newObject = dataReader.GetValue(index).ToString();
                    }
                    entitys.Add(newObject);
                }
                dataReader.Close();
                return entitys;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dataReader.Close();
            }
        }


        public static T Map(DbDataReader dataReader)
        {
            try
            {
                Type businessEntityType = typeof(T);
                Hashtable hashtable = new Hashtable();
                PropertyInfo[] properties = businessEntityType.GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    hashtable[info.Name.ToUpper()] = info;
                }

                if (dataReader.Read())
                {
                    T newObject = new T();
                    for (int index = 0; index < dataReader.FieldCount; index++)
                    {
                        PropertyInfo info = (PropertyInfo)
                                            hashtable[dataReader.GetName(index).ToUpper()];
                        if ((info != null) && info.CanWrite)
                        {
                            if (string.IsNullOrEmpty(dataReader.GetValue(index).ToString())) continue;
                            info.SetValue(newObject, dataReader.GetValue(index), null);
                        }
                    }

                    return newObject;
                }
                return new T();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                dataReader.Close();
            }
        }
    }
}
