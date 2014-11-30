using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.Utils
{
    public class PersonSkullConverter : MarkupExtension, IValueConverter
    {
        private static readonly PersonSkullConverter Converter = new PersonSkullConverter();
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Converter;
        }

        public PersonSkullConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var person = value as Person;
            return (person != null && person.DateOfDeath.HasValue) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
