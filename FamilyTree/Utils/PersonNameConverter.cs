using FamilyTree.Properties;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.Utils
{
    public class PersonNameConverter: MarkupExtension, IValueConverter
    {
        private static PersonNameConverter _converter;
        
        public PersonNameConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new PersonNameConverter());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Person)
            {
                var person = value as Person;
                return string.Format(Resources.PersonFullNameFormat, person.LastName, person.FirstName);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
