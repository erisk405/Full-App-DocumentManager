export interface DocumentProp {
  documentId: string;
  filename: string;
  original_name: string;
  description: string;
  filePath: string;
  fileType: string;
  status: string;
  createAt: string;
  createdByUserId: string;
  isDelete: boolean
}
export interface CreateDocumentDto {
  filename: string;
  original_name: string;
  description: string;
  filePath: string;
  fileType: string;
  status: string;
  createdByUserId?: string;
}

