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
                ColumnName = "CUST_NO",
                DataType = System.Type.GetType("System.String")
            };

            Columns.Add(_column);

            _column = new DataColumn
            {
                ColumnName = "CARDYN",
                DataType = System.Type.GetType("System.Char")
            };

            Columns.Add(_column);

            _column = new DataColumn
            {
                ColumnName = "NUM_CARDS",
                DataType = System.Type.GetType("System.Int32")
            };

            Columns.Add(_column);

            _column = new DataColumn
            {
                ColumnName = "IMAGE_PATH",
                DataType = System.Type.GetType("System.String")
            };

            Columns.Add(_column);
        }
    }
}