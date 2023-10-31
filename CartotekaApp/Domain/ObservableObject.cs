using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CartotekaApp.Domain
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Устанавливает значение свойства и уведомляет об изменении, если значение изменилось.
        /// </summary>
        /// <typeparam name="T">Тип значения свойства.</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свойства.</param>
        /// <param name="value">Новое значение свойства.</param>
        /// <param name="propertyName">Имя свойства (автоматически определено).</param>
        /// <returns>True, если значение свойства изменилось. Иначе - False.</returns>
        public bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Событие, возникающее при изменении свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged для указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства, для которого произошли изменения.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
