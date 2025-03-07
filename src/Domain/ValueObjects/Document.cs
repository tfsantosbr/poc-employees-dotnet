using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.ValueObjects
{
    public class Document : ValueObject
    {
        private Document(string value, DocumentType type)
        {
            Value = value;
            Type = type;
        }

        public string Value { get; }
        public DocumentType Type { get; }

        public static Result<Document> Create(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
                return Result.Failure<Document>("Document.Empty", "O documento não pode ser vazio");

            // Remove non-numeric characters
            var numericDocument = Regex.Replace(document, @"[^\d]", "");

            // Validate CPF (11 digits)
            if (numericDocument.Length == 11)
            {
                // Simple CPF validation - in production we would validate the check digits
                return Result.Success(new Document(numericDocument, DocumentType.CPF));
            }

            // Validate CNPJ (14 digits)
            if (numericDocument.Length == 14)
            {
                // Simple CNPJ validation - in production we would validate the check digits
                return Result.Success(new Document(numericDocument, DocumentType.CNPJ));
            }

            return Result.Failure<Document>("Document.InvalidFormat", "Documento inválido. Deve ser CPF (11 dígitos) ou CNPJ (14 dígitos)");
        }

        protected override object[] GetEqualityComponents()
        {
            return new object[] { Value, Type };
        }

        public override string ToString() => Value;
    }

    public enum DocumentType
    {
        CPF,
        CNPJ
    }
}