export class SavingPlan {
  stamp_entity: string;
  userId: string;
  categoryId: string;
  descricao: string;
  valorAlvo: number;
  valorAtual: number;
  dataLimite: Date;
  createdAt: Date;

  constructor() {
    this.stamp_entity = '';
    this.userId = '';
    this.categoryId = '';
    this.descricao = '';
    this.valorAlvo = 0;
    this.valorAtual = 0;
    this.dataLimite = new Date();
    this.createdAt = new Date();
  }
}

