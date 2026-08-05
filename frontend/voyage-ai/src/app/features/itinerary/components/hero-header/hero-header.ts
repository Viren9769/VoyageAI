import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-hero-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hero-header.html',
  styleUrl: './hero-header.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeroHeader {

  activeView: 'My Trips' | 'Shared With Me' = 'My Trips';

  selectView(view: 'My Trips' | 'Shared With Me'): void {
    this.activeView = view;
  }

}
