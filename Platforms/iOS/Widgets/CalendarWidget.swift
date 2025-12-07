import WidgetKit
import SwiftUI

struct CalendarWidget: Widget {
    let kind: String = "CalendarWidget"

    var body: some WidgetConfiguration {
        StaticConfiguration(kind: kind, provider: CalendarProvider()) { entry in
            CalendarWidgetEntryView(entry: entry)
        }
        .configurationDisplayName("AgendaPro Elite")
        .description("Muestra tus eventos del día")
        .supportedFamilies([.systemSmall, .systemMedium])
    }
}

struct CalendarProvider: TimelineProvider {
    func placeholder(in context: Context) -> CalendarEntry {
        CalendarEntry(date: Date(), events: [])
    }

    func getSnapshot(in context: Context, completion: @escaping (CalendarEntry) -> ()) {
        let entry = CalendarEntry(date: Date(), events: [])
        completion(entry)
    }

    func getTimeline(in context: Context, completion: @escaping (Timeline<Entry>) -> ()) {
        var entries: [CalendarEntry] = []
        let currentDate = Date()
        
        // Create entries for next 24 hours
        for hourOffset in 0..<24 {
            let entryDate = Calendar.current.date(byAdding: .hour, value: hourOffset, to: currentDate)!
            let entry = CalendarEntry(date: entryDate, events: [])
            entries.append(entry)
        }

        let timeline = Timeline(entries: entries, policy: .atEnd)
        completion(timeline)
    }
}

struct CalendarEntry: TimelineEntry {
    let date: Date
    let events: [String]
}

struct CalendarWidgetEntryView: View {
    var entry: CalendarProvider.Entry

    var body: some View {
        VStack(alignment: .leading, spacing: 4) {
            Text(entry.date, style: .date)
                .font(.headline)
            Text("Eventos del día")
                .font(.caption)
                .foregroundColor(.secondary)
        }
        .padding()
    }
}

struct CalendarWidget_Previews: PreviewProvider {
    static var previews: some View {
        CalendarWidgetEntryView(entry: CalendarEntry(date: Date(), events: []))
            .previewContext(WidgetPreviewContext(family: .systemSmall))
    }
}

