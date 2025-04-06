import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DataExportService } from '../services/data-export.service';
import { NotificationService } from '../../../shared/notifications/notification.service';

@Component({
  selector: 'app-data-export',
  templateUrl: './data-export.component.html',
  styleUrls: ['./data-export.component.scss']
})
export class DataExportComponent implements OnInit {

  @ViewChild('fileInput') fileInput!: ElementRef;

  exportForm: FormGroup;
  importForm: FormGroup;
  isLoading: boolean = false;
  importSuccess: boolean = false;
  reportTypes = [
    { value: 'monthly', label: 'Relatório Mensal' },
    { value: 'quarterly', label: 'Relatório Trimestral' },
    { value: 'yearly', label: 'Relatório Anual' },
    { value: 'category', label: 'Análise por Categoria' },
    { value: 'budget', label: 'Análise de Orçamento' }
  ];

  // Data mínima e máxima para os seletores de data
  minDate = new Date(new Date().getFullYear() - 2, 0, 1); // 2 anos atrás
  maxDate = new Date(); // Hoje

  constructor(
    private fb: FormBuilder,
    private dataExportService: DataExportService,
    private notificationService: NotificationService
  ) {
    // Inicializar formulário de exportação
    this.exportForm = this.fb.group({
      format: ['csv', Validators.required],
      startDate: [new Date(new Date().getFullYear(), new Date().getMonth() - 3, 1), Validators.required], // 3 meses atrás
      endDate: [new Date(), Validators.required],
      reportType: ['monthly']
    });

    // Inicializar formulário de importação
    this.importForm = this.fb.group({
      file: [null, Validators.required]
    });
  }

  ngOnInit(): void {
  }

  // Método para exportar dados
  exportData(): void {
    if (this.exportForm.invalid) {
      this.notificationService.showToast('error', 'Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    const formValues = this.exportForm.value;
    const startDate = formValues.startDate;
    const endDate = formValues.endDate;

    // Verificar se as datas são válidas
    if (startDate > endDate) {
      this.notificationService.showToast('error', 'A data de início deve ser anterior à data de fim.');
      return;
    }

    this.isLoading = true;

    // Exportar dados com base no formato selecionado
    if (formValues.format === 'csv') {
      this.exportCSV(startDate, endDate);
    } else if (formValues.format === 'excel') {
      this.exportExcel(startDate, endDate);
    } else if (formValues.format === 'report') {
      this.generateReport(formValues.reportType, startDate, endDate);
    }
  }

  // Exportar para CSV
  exportCSV(startDate: Date, endDate: Date): void {
    this.dataExportService.exportToCsv(startDate, endDate).subscribe(
      blob => {
        this.downloadFile(blob, 'transacoes.csv');
        this.isLoading = false;
        this.notificationService.showToast('success', 'Dados exportados com sucesso para CSV!');
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao exportar dados. Por favor, tente novamente.');
        console.error('Erro ao exportar para CSV:', error);
      }
    );
  }

  // Exportar para Excel
  exportExcel(startDate: Date, endDate: Date): void {
    this.dataExportService.exportToExcel(startDate, endDate).subscribe(
      blob => {
        this.downloadFile(blob, 'transacoes.xlsx');
        this.isLoading = false;
        this.notificationService.showToast('success', 'Dados exportados com sucesso para Excel!');
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao exportar dados. Por favor, tente novamente.');
        console.error('Erro ao exportar para Excel:', error);
      }
    );
  }

  // Gerar relatório
  generateReport(reportType: string, startDate: Date, endDate: Date): void {
    this.dataExportService.generateReport(reportType, startDate, endDate).subscribe(
      blob => {
        this.downloadFile(blob, `relatorio_${reportType}.pdf`);
        this.isLoading = false;
        this.notificationService.showToast('success', 'Relatório gerado com sucesso!');
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao gerar relatório. Por favor, tente novamente.');
        console.error('Erro ao gerar relatório:', error);
      }
    );
  }

  // Método auxiliar para download de arquivos
  downloadFile(blob: Blob, fileName: string): void {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    }, 100);
  }

  // Método para importar dados
  importData(event: any): void {
    const file = event.target.files[0];
    if (!file) return;

    // Verificar extensão do arquivo
    const fileExtension = file.name.split('.').pop()?.toLowerCase();
    if (fileExtension !== 'csv') {
      this.notificationService.showToast('error', 'Apenas arquivos CSV são suportados para importação.');
      return;
    }

    this.isLoading = true;
    this.importSuccess = false;

    this.dataExportService.importFromCsv(file).subscribe(
      response => {
        this.isLoading = false;
        this.importSuccess = true;
        this.notificationService.showToast('success', 'Dados importados com sucesso!');
        // Limpar o campo de arquivo
        this.importForm.get('file')?.setValue(null);
        // Reset o input de arquivo
        const fileInput = document.getElementById('fileInput') as HTMLInputElement;
        if (fileInput) {
          fileInput.value = '';
        }
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao importar dados. Por favor, verifique o formato do arquivo e tente novamente.');
        console.error('Erro ao importar dados:', error);
      }
    );
  }

  // Obter o label de um tipo de relatório pelo valor
  getReportTypeLabel(value: string): string {
    const reportType = this.reportTypes.find(rt => rt.value === value);
    return reportType ? reportType.label : '';
  }

  triggerFileInputClick(): void {
    this.fileInput.nativeElement.click();
  }
}
