using System;
using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class ResultVM
    {
        private Result _result;
        
        public ResultVM()
        {
            _result = new Result();
        }
        public ResultVM(Result r)
        {
            _result = r;
        }
        
        public string CursusCode
        {
            get { return _result.cursuscode; }
            set { _result.cursuscode = value; }
        }
        public string VolledigeCursusNaam
        {
            get { return _result.langeNaamCursus; }
            set { _result.langeNaamCursus = value; }
        }
        public string CursusNaam
        {
            get { return _result.korteNaamCusus; }
            set { _result.korteNaamCusus = value; }
        }
        public string Onderwerp
        {
            get { return _result.onderwerp; }
            set { _result.onderwerp = value; }
        }
        public string ToetsOmschrijving
        {
            get { return _result.toetsomschrijving; }
            set { _result.toetsomschrijving = value; }
        }
        public string HonorairePunten
        {
            get { return _result.honorairePunten; }
            set { _result.honorairePunten = value; }
        }
        public string Voldoende
        {
            get { return _result.voldoende; }
            set { _result.voldoende = value; }
        }
        public string StatusUitslag
        {
            get { return _result.status; }
            set { _result.status = value; }
        }
        public string MutatieDatumString
        {
            get { return _result.mutatiedatumTekst; }
            set { _result.mutatiedatumTekst = value; }
        }
        public DateTime MutatieDatum
        {
            get { return _result.mutatiedatum; }
            set { _result.mutatiedatum = value; }
        }
        public string ToetsDatumString
        {
            get { return _result.toetsdatumTekst; }
            set { _result.toetsdatumTekst = value; }
        }
        public DateTime ToetsDatum
        {
            get { return _result.toetsdatum; }
            set { _result.toetsdatum = value; }
        }
        public string AantalEC
        {
            get { return _result.punten; }
            set { _result.punten = value; }
        }
        public string Weging
        {
            get { return _result.weging; }
            set { _result.weging = value; }
        }

        public int Studentennummer
        {
            get { return _result.studentnummer; }
            set { _result.studentnummer = value; }
        }
        public string Docent
        {
            get { return _result.docent; }
            set { _result.docent = value; }
        }
        public string Resultaat
        {
            get { return _result.resultaat; }
            set { _result.resultaat = value; }
        }
        
    }
}
