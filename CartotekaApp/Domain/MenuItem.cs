using System;
using System.Windows.Controls;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// Пункт меню приложения.
    /// </summary>
    public class MenuItem
    {
        private readonly object _dataContext;
        public readonly Type _controlType;
        private object _page;

        /// <summary>
        /// Имя пункта меню.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Контент пункта меню.
        /// </summary>
        public object Content
        {
            get
            {
                if (_page == null)
                {
                    _page = CreatePage();
                }

                return _page;
            }
        }

        /// <summary>
        /// Создание страницы для пункта меню.
        /// </summary>
        /// <returns>Объект страницы.</returns>
        private object CreatePage()
        {
            var obj = Activator.CreateInstance(_controlType);

            if (obj is Page page)
            {
                page.DataContext = _dataContext;
            }

            return obj;
        }

        /// <summary>
        /// Конструктор класса MenuItem.
        /// </summary>
        /// <param name="name">Имя пункта меню.</param>
        /// <param name="controlType">Тип элемента управления для создания страницы.</param>
        /// <param name="dataContext">Контекст данных для привязки к странице.</param>
        public MenuItem(string name, Type controlType, object dataContext = null)
        {
            Name = name;
            _controlType = controlType;
            _dataContext = dataContext;
        }
    }
}
