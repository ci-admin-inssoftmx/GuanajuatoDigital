using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Utils
{
    public static class EntityExtensions
    {
        //public static List<T> RetrieveAllCustomers()
        //{
        //    DataTable data = new DataTable();
        //    List<Customer> customers = new List<Customer>();

        //    using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        //    {
        //        conn.Open();
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = "SELECT name, age, row_id FROM customer";

        //                DataReader reader = cmd.ExecuteReader();
        //                data.Load(reader);

        //                foreach (DataRow row in data.Rows)
        //                {
        //                    customers.Add(new Customer { Name = row.Field<string>("NAME"), Age = row.Field<int>("AGE"), row_id = row.Field<int>("ROW_ID") });
        //                }
        //            }
        //        }
        //    }

        //    return customers;
        //}



        //public static void MapDataToObject<T>(this SqlDataReader dataReader, T newObject)
        //{
        //    if (newObject == null) throw new ArgumentNullException(nameof(newObject));

        //    // Fast Member Usage
        //    var objectMemberAccessor = TypeAccessor.Create(newObject.GetType());
        //    var propertiesHashSet =
        //            objectMemberAccessor
        //            .GetMembers()
        //            .Select(mp => mp.Name)
        //            .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        //    for (int i = 0; i < dataReader.FieldCount; i++)
        //    {
        //        var name = propertiesHashSet.FirstOrDefault(a => a.Equals(dataReader.GetName(i), StringComparison.InvariantCultureIgnoreCase));
        //        if (!String.IsNullOrEmpty(name))
        //        {
        //            //Attention! if you are getting errors here, then double check that your model and sql have matching types for the field name.
        //            //Check api.log for error message!
        //            objectMemberAccessor[newObject, name]
        //                = dataReader.IsDBNull(i) ? null : dataReader.GetValue(i);
        //        }
        //    }
        //}
    }
}
