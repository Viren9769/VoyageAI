import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

import { Traveler } from '../../../../models/traveler';
import { TripOption } from '../../../../core/services/traveler.service';

@Component({
  selector: 'app-traveler-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './traveler-list.html',
  styleUrl: './traveler-list.scss',
})
export class TravelerList implements OnChanges {
  @Input() travelers: Traveler[] = [];
  @Input() tripOptions: TripOption[] = [];

  @Output() viewTraveler = new EventEmitter<Traveler>();
  @Output() editTraveler = new EventEmitter<Traveler>();
  @Output() deleteTraveler = new EventEmitter<Traveler>();

  searchTerm = '';
  selectedTripId = 'all';
  openMenuId: string | null = null;

  currentPage = 1;
  readonly pageSize = 8;

  filtered: Traveler[] = [];
  paginated: Traveler[] = [];

  ngOnChanges(_changes: SimpleChanges): void {
    this.applyFilter();
  }

  onFilterInput(): void {
    this.currentPage = 1;
    this.applyFilter();
  }

  toggleMenu(id: string, event: Event): void {
    event.stopPropagation();
    this.openMenuId = this.openMenuId === id ? null : id;
  }

  closeMenus(): void {
    this.openMenuId = null;
  }

  onView(traveler: Traveler): void {
    this.openMenuId = null;
    this.viewTraveler.emit(traveler);
  }

  onEdit(traveler: Traveler): void {
    this.openMenuId = null;
    this.editTraveler.emit(traveler);
  }

  onDelete(traveler: Traveler): void {
    this.openMenuId = null;
    this.deleteTraveler.emit(traveler);
  }

  private applyFilter(): void {
    const term = this.searchTerm.toLowerCase();
    this.filtered = this.travelers.filter((t) => {
      const fullName = `${t.firstName} ${t.lastName}`.toLowerCase();
      return fullName.includes(term) || t.email.toLowerCase().includes(term);
    });
    this.paginate();
  }

  private paginate(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    this.paginated = this.filtered.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filtered.length / this.pageSize));
  }

  get visiblePages(): number[] {
    const maxPages = 5;
    if (this.totalPages <= maxPages) {
      return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }
    const half = Math.floor(maxPages / 2);
    let start = Math.max(1, this.currentPage - half);
    const end = Math.min(this.totalPages, start + maxPages - 1);
    if (end - start + 1 < maxPages) {
      start = Math.max(1, end - maxPages + 1);
    }
    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }

  get showingFrom(): number {
    return this.filtered.length === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  get showingTo(): number {
    return Math.min(this.currentPage * this.pageSize, this.filtered.length);
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.paginate();
  }

  getInitials(traveler: Traveler): string {
    return `${traveler.firstName[0]}${traveler.lastName[0]}`.toUpperCase();
  }

  typeLabel(traveler: Traveler): string {
    return traveler.type === 'Child' ? `Child (${traveler.age}y)` : 'Adult';
  }
}
