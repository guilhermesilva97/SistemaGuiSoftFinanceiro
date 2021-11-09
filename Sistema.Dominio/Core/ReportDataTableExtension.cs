using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Dominio.Core
{
    public static class ReportDataTableExtension
    {
        public static DataTable ToDataTable<T>(this ICollection<T> source, UpperCaseConfiguration upperCaseConfiguration = null)
        {
            var pis = typeof(T).GetProperties();
            var dt = new DataTable();

            foreach (var pi in pis)
            {
                var pt = pi.PropertyType;

                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    pt = Nullable.GetUnderlyingType(pt);

                if (pt == typeof(DateTimeOffset))
                    pt = typeof(DateTime);

                if (pt == typeof(Boolean))
                    pt = typeof(string);

                if (pt.BaseType == typeof(Enum))
                    pt = typeof(int);

                dt.Columns.Add(pi.Name, pt);
            }

            foreach (var obj in source)
            {
                if (obj == null)
                    continue;

                var dr = dt.NewRow();

                foreach (var pi in pis)
                {
                    //if (pi.PropertyType.BaseType == typeof(Entidade))
                    //    continue;

                    var value = pi.GetValue(obj, null);

                    if (value == null)
                        dr[pi.Name] = DBNull.Value;
                    else if (value is DateTimeOffset)
                        dr[pi.Name] = ((DateTimeOffset)value).DateTime;
                    else if (value is bool)
                        dr[pi.Name] = (bool)value ? "Sim" : "Não";
                    else if (pi.PropertyType.BaseType == typeof(Enum))
                        dr[pi.Name] = Convert.ToInt32(value);
                    else if (value is string)
                    {
                        if (upperCaseConfiguration != null)
                        {
                            if (upperCaseConfiguration.Exceptions.Any(s => s == pi.Name))
                                dr[pi.Name] = value;
                            else
                                dr[pi.Name] = ((string)value).ToUpper();
                        }
                        else
                        {
                            dr[pi.Name] = value;
                        }
                    }
                    else
                        dr[pi.Name] = value;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        //public static DataTable ToDataTable(this EntityObject source)
        //{

        //    var elementType = source.GetType();
        //    var pis = elementType.GetProperties();
        //    var dt = new DataTable();

        //    foreach (var pi in pis)
        //    {
        //        var pt = pi.PropertyType;

        //        if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
        //            pt = Nullable.GetUnderlyingType(pt);

        //        if (pt == typeof(DateTimeOffset))
        //            pt = typeof(DateTime);

        //        if (pt == typeof(Boolean))
        //            pt = typeof(string);

        //        dt.Columns.Add(pi.Name, pt);
        //    }

        //    var dr = dt.NewRow();

        //    foreach (var pi in pis)
        //    {
        //        var value = pi.GetValue(source, null);

        //        if (value == null)
        //            dr[pi.Name] = DBNull.Value;
        //        else if (value is DateTimeOffset)
        //            dr[pi.Name] = ((DateTimeOffset)value).DateTime;
        //        else if (value is bool)
        //            dr[pi.Name] = (bool)value ? "Sim" : "Não";
        //        else
        //            dr[pi.Name] = value;
        //    }

        //    dt.Rows.Add(dr);

        //    return dt;
        //}
    }

    public class UpperCaseConfiguration
    {
        public bool Uppercase { get; set; }
        public List<string> Exceptions { get; set; }

        public UpperCaseConfiguration(bool uppercase, IEnumerable<string> exceptions = null)
        {
            Uppercase = uppercase;
            Exceptions = new List<string>();
            if (exceptions != null)
                Exceptions = exceptions.ToList();
        }
    }
}
