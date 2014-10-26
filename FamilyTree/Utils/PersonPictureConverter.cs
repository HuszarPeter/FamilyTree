using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using FamilyTree.Properties;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.Utils
{
    public class PersonPictureConverter : MarkupExtension, IValueConverter
    {
        private PersonPictureConverter _converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new PersonPictureConverter());
        }

        public PersonPictureConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Person)) return value;

            var person = value as Person;
            var result = new BitmapImage();
            result.BeginInit();
            if (person.Picture != null && person.Picture.Length > 0)
            {
                result.StreamSource = new MemoryStream(person.Picture);
            }
            else
            {
                var resourceName = person.Gender == Gender.Male ? "male" : "female";
                result.UriSource = new Uri(string.Format("pack://application:,,,/Res/{0}.png", resourceName));
            }
            result.EndInit();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
