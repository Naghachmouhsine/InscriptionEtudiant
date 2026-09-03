namespace InscriptionEtudiant.ViewModels.Admin
{
    public class ParcoursAcademiqueViewModel
    {
        public int AnneeBac { get; set; }

        public decimal? NoteNationale { get; set; }

        public decimal? NoteRegionale { get; set; }

        public string SerieBac { get; set; } = string.Empty;

        public string Mention { get; set; } = string.Empty;
    }
}
