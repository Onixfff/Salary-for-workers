using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salary_for_workers
{
    public class DateSetMainForm
    {
        public string TableName { get; private set; }
        public string Text {  get; private set; }

        public DateSetMainForm(string tableName, string text)
        {
            TableName = tableName;
            Text = text;
        }
    }
}
