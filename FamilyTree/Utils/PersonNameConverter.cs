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
            if (!(value is Person)) return value;
            
            var person = value as Person;
            return (parameter == null)
                ? string.Format(Resources.PersonFullNameFormat, person.LastName, person.FirstName)
                : string.Format(Resources.PersonFullNameFormat, person.BirthLastName, person.BirthFirstName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string GetFullNameOfPerson(Person person)
        {
            return string.Format("{0}",
                _converter.Convert(person, typeof (string), null, CultureInfo.CurrentUICulture));
        }
    }
}
