


using System;

public enum DocumentType{ IDCard =0,InternetHistory,BankAccount,CriminalRecord,Script,None }

public class LastTouchedDocument : MonoSingleton<LastTouchedDocument>
{
    private DocumentType lastTouchedDocument = DocumentType.None;
    public Action<DocumentType> OnSelectedDocumentChanged;
    public void NotifyTouchedDocument(DocumentType _docType)
    {
        if (_docType != lastTouchedDocument)
        {
            lastTouchedDocument = _docType;   
            OnSelectedDocumentChanged?.Invoke(lastTouchedDocument);
        }   
        
    }

    public DocumentType GetLastDocumentTouched() => lastTouchedDocument;
}
