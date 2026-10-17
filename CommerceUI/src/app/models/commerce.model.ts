export interface UploadResponseDto {
  message: string;
  totalRecords: number;
}

export interface ProcessResponseDto {
  recordsInQuarantine: number;
  message: string;
}

export interface CommerceQuarantineDto {
  id: number;
  pcNomcomred: string | null;
  pcNumdoc: string | null;
  pcProcessdate: string;
  motivo: string;
  fechaCuarentena: string;
}