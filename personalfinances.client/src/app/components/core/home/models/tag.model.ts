export interface Tag {
  stampEntity: string;
  userId: string;
  name: string;
  color: string;
}

export interface TransactionTag {
  stampEntity: string;
  transactionId: string;
  tagId: string;
}
