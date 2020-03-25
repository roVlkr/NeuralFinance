using NeuralFinance.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NeuralFinance.View
{
    public class Form : UserControl
    {
        #region Static members
        public static readonly DependencyProperty ViewModelProperty;
        public static readonly DependencyProperty SubmitCommandProperty;
        public static readonly DependencyProperty SubmitExecutedProperty;
        public static readonly DependencyProperty SubmitCanExecuteProperty;
        private static readonly DependencyPropertyKey SubmitCommandBindingPropertyKey;
        public static readonly DependencyProperty SubmitCommandBindingPropery;

        static Form()
        {
            ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ViewModelBase),
                typeof(Form), new PropertyMetadata(null));
            SubmitCommandProperty = DependencyProperty.Register(nameof(SubmitCommand), typeof(RoutedUICommand),
                typeof(Form), new PropertyMetadata(null, SubmitCommand_PropertyChanged));
            SubmitExecutedProperty = DependencyProperty.Register(nameof(SubmitExecuted), typeof(ExecutedRoutedEventHandler),
                typeof(Form), new PropertyMetadata(null, SubmitExecuted_PropertyChanged));
            SubmitCanExecuteProperty = DependencyProperty.Register(nameof(SubmitCanExecute), typeof(CanExecuteRoutedEventHandler),
                typeof(Form), new PropertyMetadata(null, SubmitCanExecute_PropertyChanged));

            SubmitCommandBindingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SubmitCommandBinding),
                typeof(CommandBinding), typeof(Form), new PropertyMetadata(null, SubmitCommandBinding_PropertyChanged));
            SubmitCommandBindingPropery = SubmitCommandBindingPropertyKey.DependencyProperty;
        }

        private static void SubmitCommand_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Form form)
            {
                form.SubmitCommandBinding = new CommandBinding((ICommand)e.NewValue,
                    form.SubmitExecuted, form.SubmitCanExecute);
            }
        }

        private static void SubmitExecuted_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Form form)
            {
                form.SubmitCommandBinding = new CommandBinding(form.SubmitCommand,
                    (ExecutedRoutedEventHandler)e.NewValue, form.SubmitCanExecute);
            }
        }

        private static void SubmitCanExecute_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Form form)
            {
                form.SubmitCommandBinding = new CommandBinding(form.SubmitCommand,
                    form.SubmitExecuted, (CanExecuteRoutedEventHandler)e.NewValue);
            }
        }

        private static void SubmitCommandBinding_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Form form)
            {
                if (e.OldValue != null)
                    form.CommandBindings.Remove((CommandBinding)e.OldValue);

                form.CommandBindings.Add((CommandBinding)e.NewValue);
            }
        }
        #endregion

        // Gives the amount of errors per control
        protected readonly Dictionary<IInputElement, int> validationErrors;

        public Form()
        {
            validationErrors = new Dictionary<IInputElement, int>();

            SetBinding(DataContextProperty, new Binding(nameof(ViewModel)) { Source = this });
            Validation.AddErrorHandler(this, TextBox_Error);
        }

        public bool HasError => validationErrors?.Any(entry => entry.Value != 0) ?? true;

        public ViewModelBase ViewModel
        {
            get { return (ViewModelBase)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public RoutedUICommand SubmitCommand
        {
            get { return (RoutedUICommand)GetValue(SubmitCommandProperty); }
            set { SetValue(SubmitCommandProperty, value); }
        }

        public CommandBinding SubmitCommandBinding
        {
            get { return (CommandBinding)GetValue(SubmitCommandBindingPropery); }
            protected set { SetValue(SubmitCommandBindingPropertyKey, value); }
        }

        public ExecutedRoutedEventHandler SubmitExecuted
        {
            get { return (ExecutedRoutedEventHandler)GetValue(SubmitExecutedProperty); }
            set { SetValue(SubmitExecutedProperty, value); }
        }

        public CanExecuteRoutedEventHandler SubmitCanExecute
        {
            get { return (CanExecuteRoutedEventHandler)GetValue(SubmitCanExecuteProperty); }
            set { SetValue(SubmitCanExecuteProperty, value); }
        }

        protected void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            if (!(e.OriginalSource is TextBox textBox))
                return;

            if (validationErrors.ContainsKey(textBox))
            {
                switch (e.Action)
                {
                    case ValidationErrorEventAction.Added:
                        validationErrors[textBox]++;
                        break;
                    case ValidationErrorEventAction.Removed:
                        validationErrors[textBox]--;
                        break;
                }
            }
            else
            {
                // e.Action must be "Added"
                validationErrors.Add(textBox, 1);
            }
        }
    }
}
