import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export type HeroView = 'My Trips' | 'Shared With Me';

@Component({
  selector: 'app-hero-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hero-header.html',
  styleUrl: './hero-header.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeroHeader {

  @Input()
  activeView: HeroView = 'My Trips';

  @Output()
  viewChanged = new EventEmitter<HeroView>();

  selectView(view: HeroView): void {
    if (this.activeView === view) {
      return;
    }

    this.activeView = view;
    this.viewChanged.emit(view);
  }

}
