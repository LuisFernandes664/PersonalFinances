import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DateFormatPipe } from './helpers/date-format.pipe';
import { TruncatePipe } from './helpers/truncate.pipe';

@NgModule({
  declarations: [
    TruncatePipe,
    DateFormatPipe
  ],
  imports: [CommonModule],
  exports: [
    DateFormatPipe,
    TruncatePipe
  ]
})
export class PipeModule {}
