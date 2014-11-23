using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using FamilyTree.Dal.Model;

namespace FamilyTree.Utils
{
    public class AgeKeyToTextConverter : MarkupExtension, IValueConverter
    {
        private static AgeKeyToTextConverter _converter ;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new AgeKeyToTextConverter());
        }

        public AgeKeyToTextConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AgeKey)
            {
                var a = (AgeKey) value;
                switch (a)
                {
                    case AgeKey.Child:
                        return Properties.Resources.AgeKeyChildren;
                    case AgeKey.Teen:
                        return Properties.Resources.AgeKeyTeen;
                    case AgeKey.Adult:
                        return Properties.Resources.AgeKeyAdult;
                    case AgeKey.UnknownAge:
                        return Properties.Resources.AgeKeyUnknown;
                    case AgeKey.Sum:
                        return Properties.Resources.AgeKeySum;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
