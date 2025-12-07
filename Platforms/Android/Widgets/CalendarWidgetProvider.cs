using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace AgendaProElite.Platforms.Android.Widgets;

[BroadcastReceiver(Label = "AgendaPro Elite Widget")]
[IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
[MetaData("android.appwidget.provider", Resource = "@xml/calendar_widget_info")]
public class CalendarWidgetProvider : AppWidgetProvider
{
    public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
    {
        // Update widget with today's events
        foreach (var widgetId in appWidgetIds)
        {
            var views = new RemoteViews(context.PackageName, Resource.Layout.calendar_widget);
            
            // Set today's date
            views.SetTextViewText(Resource.Id.widget_date, DateTime.Now.ToString("dd MMM yyyy"));
            
            // Update events list (simplified - would fetch from database)
            views.SetTextViewText(Resource.Id.widget_events, "Cargando eventos...");
            
            appWidgetManager.UpdateAppWidget(widgetId, views);
        }
    }
}

