using System.Data;

namespace FetchDataFromDynamics
{
    public class ContactTable : DataTable
    {
        private readonly DataColumn _column;

        public ContactTable()
        {
            _column = new DataColumn
            {
                ColumnName = "ContactId",
                DataType = System.Type.GetType("System.String")
            };

            Columns.Add(_column);

            _column = new DataColumn
            {
                ColumnName = "FirstName",
                DataType = System.Type.GetType("System.String")
            };

            Columns.Add(_column);

            _column = new DataColumn
            {
                ColumnName = "LastName",
                DataType = System.Type.GetType("System.String")
            };

            Columns.Add(_column);

            _column = new DataColumn
            {
                ColumnName = "CreatedInDynamicsOn",
                DataType = System.Type.GetType("System.DateTime")
            };

            Columns.Add(_column);
        }
    }
}