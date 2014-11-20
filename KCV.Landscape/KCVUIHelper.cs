﻿using Grabacr07.KanColleViewer.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Gizeta.KCV.Landscape
{
    public static class KCVUIHelper
    {
        public static MainWindow KCVWindow { get; private set; }

        private static Task task;

        private static Action<Task> getMainWindow = t =>
        {
            if (KCVWindow != null) return;
            KCVWindow = MainWindow.Current;
            Task.Delay(500).ContinueWith(t2 => getMainWindow(t2));
        };

        static KCVUIHelper()
        {
            task = Task.Delay(500).ContinueWith(getMainWindow);
        }

        public static void OperateMainWindow(Action operation)
        {
            task = task.ContinueWith(t =>
            {
                KCVWindow.Dispatcher.Invoke(operation);
            });
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (object childObj in LogicalTreeHelper.GetChildren(depObj))
                {
                    DependencyObject child = childObj as DependencyObject;
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindLogicalChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static IEnumerable<T> FindChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (DependencyObject child in GetChildObjects(depObj))
                {
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach (T descendant in FindChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        public static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent)
        {
            if (parent == null) yield break;
            if(parent is ContentElement || parent is FrameworkElement)
            {
                foreach(object childObj in LogicalTreeHelper.GetChildren(parent))
                {
                    var child = childObj as DependencyObject;
                    if (child != null)
                        yield return (DependencyObject)child;
                }
            }
            else
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(parent, i);
                }                
            }
        }
    }
}