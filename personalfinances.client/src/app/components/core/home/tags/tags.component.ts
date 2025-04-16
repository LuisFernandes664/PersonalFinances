import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { TagDetailsDialogComponent } from './tag-details-dialog/tag-details-dialog.component';
import { forkJoin, of } from 'rxjs';
import { catchError, finalize, map } from 'rxjs/operators';
import { Tag } from './tag.model';
import { TagService } from './tag.service';
import { Transaction } from '../models/transaction.model';

// Interface para armazenar as métricas de tag separadamente
interface TagMetrics {
  tagId: string;
  transactionCount: number;
  totalAmount: number;
  isLoading: boolean;
}

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.scss']
})
export class TagsComponent implements OnInit {
  tags: Tag[] = [];
  tagMetrics: TagMetrics[] = []; // Armazena métricas separadamente
  isLoading: boolean = false;
  searchForm: FormGroup;
  createTagForm: FormGroup;
  showCreateForm: boolean = false;

  // Cores predefinidas para seleção no formulário
  predefinedColors: string[] = [
    '#f44336', '#e91e63', '#9c27b0', '#673ab7',
    '#3f51b5', '#2196f3', '#03a9f4', '#00bcd4',
    '#009688', '#4caf50', '#8bc34a', '#cddc39',
    '#ffeb3b', '#ffc107', '#ff9800', '#ff5722'
  ];

  constructor(
    private tagService: TagService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private notificationService: NotificationService
  ) {
    this.searchForm = this.fb.group({
      searchTerm: ['']
    });

    this.createTagForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(30)]],
      color: ['#2196f3', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadTags();

    // Subscribe to search form changes
    this.searchForm.get('searchTerm')?.valueChanges.subscribe(term => {
      this.filterTags(term);
    });
  }

  loadTags(): void {
    this.isLoading = true;
    this.tagService.getTags().subscribe(
      response => {
        this.tags = response.data;

        // Inicializa as métricas para cada tag
        this.tagMetrics = this.tags.map(tag => ({
          tagId: tag.stampEntity,
          transactionCount: 0,
          totalAmount: 0,
          isLoading: true
        }));

        this.isLoading = false;

        // Após carregar as tags, busca as métricas para cada uma
        this.loadTagMetrics();
      },
      error => {
        this.notificationService.showToast('error', 'Erro ao carregar tags. Por favor, tente novamente.');
        this.isLoading = false;
        console.error('Erro ao carregar tags:', error);
      }
    );
  }

  // Método para obter métricas de uma tag específica
  getMetricForTag(tag: Tag): TagMetrics | undefined {
    return this.tagMetrics.find(metric => metric.tagId === tag.stampEntity);
  }

  // Método para carregar métricas de cada tag
  loadTagMetrics(): void {
    this.tagMetrics.forEach(metric => {
      metric.isLoading = true;

      this.tagService.getTransactionsByTag(metric.tagId).pipe(
        catchError(error => {
          console.error(`Erro ao carregar transações para tag ${metric.tagId}:`, error);
          return of({ data: [], success: false, message: 'Erro ao carregar transações' });
        }),
        finalize(() => {
          metric.isLoading = false;
        })
      ).subscribe(response => {
        if (response && response.data) {
          // Define o contador de transações
          metric.transactionCount = response.data.length;

          // Calcula o total de transações - Corrigido aqui
          metric.totalAmount = (response.data as Transaction[]).reduce((sum: number, transaction: Transaction) => {
            // Verifique se a transação tem a propriedade amount
            if (transaction && typeof transaction.amount === 'number') {
              return sum + transaction.amount;
            }
            return sum;
          }, 0);
        }
      });
    });
  }

  filterTags(searchTerm: string): void {
    // Implementar lógica de filtragem aqui se necessário
    // Por enquanto, isso está sendo feito no template com pipe
  }

  showAddTagForm(): void {
    this.showCreateForm = true;
    this.createTagForm.reset({
      name: '',
      color: '#2196f3'
    });
  }

  cancelAddTag(): void {
    this.showCreateForm = false;
  }

  createTag(): void {
    if (this.createTagForm.valid) {
      this.isLoading = true;

      const newTag: Tag = {
        stampEntity: '', // Será preenchido pelo backend
        userId: '',      // Será preenchido pelo backend com o usuário autenticado
        name: this.createTagForm.value.name,
        color: this.createTagForm.value.color
      };

      this.tagService.createTag(newTag).subscribe(
        response => {
          this.notificationService.showToast('success', 'Tag criada com sucesso!');
          this.loadTags();
          this.showCreateForm = false;
        },
        error => {
          this.notificationService.showToast('error', 'Erro ao criar tag. Por favor, tente novamente.');
          this.isLoading = false;
          console.error('Erro ao criar tag:', error);
        }
      );
    }
  }

  openTagDetails(tag: Tag): void {
    const dialogRef = this.dialog.open(TagDetailsDialogComponent, {
      width: '500px',
      data: { tag: tag }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.action === 'edit') {
          this.updateTag(result.tag);
        } else if (result.action === 'delete') {
          this.deleteTag(result.tag.stampEntity);
        }
      }
    });
  }

  updateTag(tag: Tag): void {
    this.isLoading = true;
    this.tagService.updateTag(tag.stampEntity, tag).subscribe(
      response => {
        this.notificationService.showToast('success', 'Tag atualizada com sucesso!');
        this.loadTags();
      },
      error => {
        this.notificationService.showToast('error', 'Erro ao atualizar tag. Por favor, tente novamente.');
        this.isLoading = false;
        console.error('Erro ao atualizar tag:', error);
      }
    );
  }

  deleteTag(tagId: string): void {
    this.isLoading = true;
    this.tagService.deleteTag(tagId).subscribe(
      response => {
        this.notificationService.showToast('success', 'Tag excluída com sucesso!');
        this.loadTags();
      },
      error => {
        this.notificationService.showToast('error', 'Erro ao excluir tag. Por favor, tente novamente.');
        this.isLoading = false;
        console.error('Erro ao excluir tag:', error);
      }
    );
  }

  // Formatar valor para exibição de moeda
  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-PT', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(value);
  }

  // Helper para verificar contraste e definir texto branco ou preto dependendo da cor de fundo
  getTextColor(backgroundColor: string): string {
    // Convertendo o hex para RGB
    const hex = backgroundColor.replace('#', '');
    const r = parseInt(hex.substr(0, 2), 16);
    const g = parseInt(hex.substr(2, 2), 16);
    const b = parseInt(hex.substr(4, 2), 16);

    // Calculando a luminosidade
    // Fórmula para luminosidade perceptiva: 0.299*R + 0.587*G + 0.114*B
    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

    // Se luminância for > 0.5, usar texto preto, senão usar branco
    return luminance > 0.5 ? '#000000' : '#FFFFFF';
  }
}
