﻿using LiteDB;
using SnapNotes.Models;
using SnapNotes.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SnapNotes.Repositories
{
    public class NoteRepository : INoteRepository
    {
        ILiteCollection<CaseNote> casenotes;

        public NoteRepository()
        {
            var appPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
            "Data");
            var database = new LiteDatabase(appPath);
            casenotes = database.GetCollection<CaseNote>("CaseNotes") as LiteCollection<CaseNote>;
        }

        public IEnumerable<CaseNote> ReturnAll()
        {
            return casenotes.FindAll();
        }

        public IEnumerable<CaseNote> ReturnByDateTime(DateTimeOffset? startTime, DateTimeOffset? endTime)
        {
            IEnumerable<CaseNote> notes;
            notes = casenotes.Find(x => x.StartTime < endTime && startTime < x.EndTime);
            return notes;
        }

        public IEnumerable<CaseNote> ReturnByTimeSpan(TimeSpan startTime, TimeSpan endTime, IEnumerable<CaseNote> filtered = null)
        {
            IEnumerable<CaseNote> notes;
            if (filtered == null)
            {
                notes = casenotes.Find(x => x.StartTime.TimeOfDay < endTime && startTime < x.EndTime.TimeOfDay);
            }
            else
            {
                notes = filtered.Where((x) =>
                { return x.StartTime.TimeOfDay < endTime && startTime < x.EndTime.TimeOfDay; }).ToList();
            }
            return notes;
        }

        public IEnumerable<CaseNote> ReturnOverlapping(IEnumerable<CaseNote> notes)
        {
            var overlapping = new List<CaseNote>();
            overlapping = (from first in notes
                           from second in notes
                           where first.StartTime < second.EndTime
                           && second.StartTime < first.EndTime
                           && first != second
                           select first).ToList();
            return overlapping;
        }

        public Boolean SubmitNote(CaseNote caseNote)
        {
            casenotes.Insert(caseNote);
            return true;
        }
    }
}
