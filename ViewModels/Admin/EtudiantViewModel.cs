namespace InscriptionEtudiant.ViewModels.Admin
{
    public class EtudiantViewModel
    {
        public int Id { get; set; }

        public string CNE { get; set; } = string.Empty;

        public string Nom { get; set; } = string.Empty;

        public string Prenom { get; set; } = string.Empty;

        public string NomComplet { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telephone { get; set; } = string.Empty;
    }
}
