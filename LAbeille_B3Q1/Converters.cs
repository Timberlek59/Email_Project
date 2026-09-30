using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LAbeille_B3Q1
{
    public class TaskColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDone && isDone)
            {
                return Brushes.Gray; // Tâche terminée
            }
            return Brushes.Black; // Tâche non terminée
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Convertisseur pour barrer le texte si la tâche est terminée
    public class TaskDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDone && isDone)
            {
                return TextDecorations.Strikethrough; // Texte barré
            }
            return null; // Pas de décoration
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Convertisseur pour modifier le style (ex: mettre en italien/italic si terminée)
    public class TaskStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDone && isDone)
            {
                return FontStyles.Italic; // Texte en italique
            }
            return FontStyles.Normal; // Texte normal
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
