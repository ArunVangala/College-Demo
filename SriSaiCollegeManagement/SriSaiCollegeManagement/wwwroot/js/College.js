// wwwroot/js/site.js

// Global Application Object
const SriSaiApp = {
    init: function () {
        this.initializeComponents();
        this.bindEvents();
        this.setupAnimations();
    },

    initializeComponents: function () {
        // Initialize tooltips
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });

        // Initialize popovers
        var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });

        // Auto-hide alerts after 5 seconds
        setTimeout(function () {
            $('.alert').fadeOut('slow');
        }, 5000);
    },

    bindEvents: function () {
        // Form validation
        this.setupFormValidation();

        // Search functionality
        this.setupSearch();

        // Attendance marking
        this.setupAttendanceMarking();

        // Result entry
        this.setupResultEntry();

        // Dashboard interactions
        this.setupDashboardInteractions();
    },

    setupAnimations: function () {
        // Animate elements on scroll
        this.animateOnScroll();

        // Loading animations
        this.setupLoadingStates();
    },

    // Form Validation
    setupFormValidation: function () {
        // Custom validation styles
        (function () {
            'use strict';
            window.addEventListener('load', function () {
                var forms = document.getElementsByClassName('needs-validation');
                var validation = Array.prototype.filter.call(forms, function (form) {
                    form.addEventListener('submit', function (event) {
                        if (form.checkValidity() === false) {
                            event.preventDefault();
                            event.stopPropagation();
                        }
                        form.classList.add('was-validated');
                    }, false);
                });
            }, false);
        })();

        // Password strength indicator
        $('#Password').on('keyup', function () {
            const password = $(this).val();
            const strength = SriSaiApp.checkPasswordStrength(password);
            SriSaiApp.updatePasswordStrength(strength);
        });

        // Email availability check
        $('#Email').on('blur', function () {
            const email = $(this).val();
            if (email) {
                SriSaiApp.checkEmailAvailability(email);
            }
        });
    },

    checkPasswordStrength: function (password) {
        let strength = 0;
        if (password.length >= 8) strength++;
        if (/[a-z]/.test(password)) strength++;
        if (/[A-Z]/.test(password)) strength++;
        if (/[0-9]/.test(password)) strength++;
        if (/[^A-Za-z0-9]/.test(password)) strength++;
        return strength;
    },

    updatePasswordStrength: function (strength) {
        const progressBar = $('#password-strength');
        const strengthText = $('#password-strength-text');

        let percentage = (strength / 5) * 100;
        let className = '';
        let text = '';

        if (strength <= 1) {
            className = 'bg-danger';
            text = 'Very Weak';
        } else if (strength <= 2) {
            className = 'bg-warning';
            text = 'Weak';
        } else if (strength <= 3) {
            className = 'bg-info';
            text = 'Medium';
        } else if (strength <= 4) {
            className = 'bg-primary';
            text = 'Strong';
        } else {
            className = 'bg-success';
            text = 'Very Strong';
        }

        progressBar.removeClass('bg-danger bg-warning bg-info bg-primary bg-success')
            .addClass(className)
            .css('width', percentage + '%');

        strengthText.text(text);
    },

    checkEmailAvailability: function (email) {
        // Simulated email check - replace with actual AJAX call
        const commonEmails = ['admin@gmail.com', 'test@test.com'];
        const emailInput = $('#Email');
        const feedback = emailInput.next('.invalid-feedback');

        if (commonEmails.includes(email)) {
            emailInput.addClass('is-invalid');
            feedback.text('This email is already registered.');
        } else {
            emailInput.removeClass('is-invalid').addClass('is-valid');
        }
    },

    // Search Functionality
    setupSearch: function () {
        $('#searchInput').on('input', function () {
            const searchTerm = $(this).val().toLowerCase();
            SriSaiApp.filterTableRows(searchTerm);
        });

        // Advanced search toggle
        $('#advancedSearchToggle').on('click', function () {
            $('#advancedSearchPanel').slideToggle();
        });
    },

    filterTableRows: function (searchTerm) {
        $('.searchable-table tbody tr').each(function () {
            const row = $(this);
            const text = row.text().toLowerCase();

            if (text.indexOf(searchTerm) > -1) {
                row.show().addClass('search-highlight');
                setTimeout(() => row.removeClass('search-highlight'), 2000);
            } else {
                row.hide();
            }
        });

        // Show "no results" message if needed
        const visibleRows = $('.searchable-table tbody tr:visible').length;
        if (visibleRows === 0 && searchTerm !== '') {
            if ($('.no-results').length === 0) {
                $('.searchable-table tbody').append('<tr class="no-results"><td colspan="100%" class="text-center text-muted py-4">No results found</td></tr>');
            }
        } else {
            $('.no-results').remove();
        }
    },

    // Attendance Marking
    setupAttendanceMarking: function () {
        $('#SubjectId').on('change', function () {
            const subjectId = $(this).val();
            const date = $('#Date').val();

            if (subjectId && date) {
                SriSaiApp.loadStudentsForAttendance(subjectId, date);
            }
        });

        $('#Date').on('change', function () {
            const subjectId = $('#SubjectId').val();
            const date = $(this).val();

            if (subjectId && date) {
                SriSaiApp.loadStudentsForAttendance(subjectId, date);
            }
        });

        // Bulk attendance actions
        $('#markAllPresent').on('click', function () {
            $('.attendance-checkbox').prop('checked', true);
        });

        $('#markAllAbsent').on('click', function () {
            $('.attendance-checkbox').prop('checked', false);
        });
    },

    loadStudentsForAttendance: function (subjectId, date) {
        $.ajax({
            url: '/Teacher/GetStudentsForSubject',
            method: 'GET',
            data: { subjectId: subjectId, date: date },
            beforeSend: function () {
                $('#studentsContainer').html('<div class="text-center py-4"><div class="spinner-border text-primary" role="status"></div></div>');
            },
            success: function (data) {
                let html = '';
                if (data.length > 0) {
                    html += '<div class="row mb-3"><div class="col-12">';
                    html += '<button type="button" id="markAllPresent" class="btn btn-success btn-sm me-2">Mark All Present</button>';
                    html += '<button type="button" id="markAllAbsent" class="btn btn-danger btn-sm">Mark All Absent</button>';
                    html += '</div></div>';

                    data.forEach(function (student, index) {
                        html += '<div class="card mb-2">';
                        html += '<div class="card-body py-2">';
                        html += '<div class="row align-items-center">';
                        html += '<div class="col-md-6">';
                        html += '<h6 class="mb-0">' + student.studentName + '</h6>';
                        html += '<small class="text-muted">ID: ' + student.studentId_Display + '</small>';
                        html += '</div>';
                        html += '<div class="col-md-3">';
                        html += '<div class="form-check form-switch">';
                        html += '<input class="form-check-input attendance-checkbox" type="checkbox" name="StudentAttendances[' + index + '].IsPresent" id="student_' + student.studentId + '" ' + (student.isPresent ? 'checked' : '') + '>';
                        html += '<label class="form-check-label" for="student_' + student.studentId + '">Present</label>';
                        html += '</div>';
                        html += '</div>';
                        html += '<div class="col-md-3">';
                        html += '<input type="text" class="form-control form-control-sm" name="StudentAttendances[' + index + '].Remarks" placeholder="Remarks" value="' + (student.remarks || '') + '">';
                        html += '</div>';
                        html += '<input type="hidden" name="StudentAttendances[' + index + '].StudentId" value="' + student.studentId + '">';
                        html += '</div>';
                        html += '</div>';
                        html += '</div>';
                    });
                } else {
                    html = '<div class="alert alert-info">No students found for this subject.</div>';
                }

                $('#studentsContainer').html(html);

                // Re-bind events
                SriSaiApp.setupAttendanceMarking();
            },
            error: function () {
                $('#studentsContainer').html('<div class="alert alert-danger">Error loading students. Please try again.</div>');
            }
        });
    },

    // Result Entry
    setupResultEntry: function () {
        $('.marks-input').on('input', function () {
            const input = $(this);
            const marks = parseInt(input.val()) || 0;
            const totalMarks = parseInt($('#totalMarks').val()) || 100;
            const passMarks = parseInt($('#passMarks').val()) || 40;

            // Calculate percentage and grade
            const percentage = (marks / totalMarks) * 100;
            const grade = SriSaiApp.calculateGrade(percentage);
            const isPassed = marks >= passMarks;

            // Update grade display
            const row = input.closest('tr');
            row.find('.grade-display').text(grade);
            row.find('.grade-input').val(grade);
            row.find('.passed-input').val(isPassed);

            // Update row styling
            if (isPassed) {
                row.removeClass('table-danger').addClass('table-success');
            } else {
                row.removeClass('table-success').addClass('table-danger');
            }
        });

        // Grade calculation based on percentage
        this.calculateGrade = function (percentage) {
            if (percentage >= 90) return 'A+';
            if (percentage >= 80) return 'A';
            if (percentage >= 70) return 'B+';
            if (percentage >= 60) return 'B';
            if (percentage >= 50) return 'C+';
            if (percentage >= 40) return 'C';
            return 'F';
        };
    },

    // Dashboard Interactions
    setupDashboardInteractions: function () {
        // Animate counter numbers
        $('.counter').each(function () {
            const $this = $(this);
            const target = parseInt($this.text());
            $this.text('0');

            $({ count: 0 }).animate({ count: target }, {
                duration: 2000,
                easing: 'swing',
                step: function () {
                    $this.text(Math.floor(this.count));
                },
                complete: function () {
                    $this.text(target);
                }
            });
        });

        // Chart.js integration for dashboard charts
        if (typeof Chart !== 'undefined') {
            this.initializeDashboardCharts();
        }

        // Real-time notifications (simulated)
        this.setupRealTimeNotifications();
    },

    initializeDashboardCharts: function () {
        // Attendance Chart
        const attendanceCtx = document.getElementById('attendanceChart');
        if (attendanceCtx) {
            new Chart(attendanceCtx, {
                type: 'doughnut',
                data: {
                    labels: ['Present', 'Absent'],
                    datasets: [{
                        data: [85, 15],
                        backgroundColor: ['#28a745', '#dc3545'],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });
        }

        // Results Chart
        const resultsCtx = document.getElementById('resultsChart');
        if (resultsCtx) {
            new Chart(resultsCtx, {
                type: 'bar',
                data: {
                    labels: ['A+', 'A', 'B+', 'B', 'C+', 'C', 'F'],
                    datasets: [{
                        label: 'Number of Students',
                        data: [12, 19, 15, 8, 5, 3, 1],
                        backgroundColor: [
                            '#28a745', '#20c997', '#17a2b8',
                            '#007bff', '#6f42c1', '#fd7e14', '#dc3545'
                        ],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: false
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                stepSize: 1
                            }
                        }
                    }
                }
            });
        }
    },

    setupRealTimeNotifications: function () {
        // Simulate real-time notifications
        setInterval(function () {
            // This would be replaced with actual SignalR or WebSocket implementation
            const notifications = [
                'New assignment submitted by John Doe',
                'Exam scheduled for next week',
                'Library book due date reminder',
                'Fee payment deadline approaching'
            ];

            if (Math.random() < 0.1) { // 10% chance every interval
                const notification = notifications[Math.floor(Math.random() * notifications.length)];
                SriSaiApp.showNotification(notification, 'info');
            }
        }, 30000); // Check every 30 seconds
    },

    showNotification: function (message, type = 'info') {
        const notification = $(`
            <div class="toast align-items-center text-white bg-${type} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="fas fa-bell me-2"></i>${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `);

        $('#notificationContainer').append(notification);
        const toast = new bootstrap.Toast(notification[0]);
        toast.show();
    },

    // Animation on Scroll
    animateOnScroll: function () {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -100px 0px'
        };

        const observer = new IntersectionObserver(function (entries) {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');
                }
            });
        }, observerOptions);

        // Observe elements with animation classes
        document.querySelectorAll('.animate-on-scroll').forEach(el => {
            observer.observe(el);
        });
    },

    setupLoadingStates: function () {
        // Add loading states to forms
        $('form').on('submit', function () {
            const submitBtn = $(this).find('button[type="submit"]');
            const originalText = submitBtn.text();

            submitBtn.prop('disabled', true)
                .html('<span class="spinner-border spinner-border-sm me-2"></span>Processing...');

            // Reset after 5 seconds if form hasn't navigated away
            setTimeout(function () {
                submitBtn.prop('disabled', false).text(originalText);
            }, 5000);
        });

        // Add loading states to AJAX requests
        $(document).ajaxStart(function () {
            $('.loading-overlay').fadeIn();
        }).ajaxStop(function () {
            $('.loading-overlay').fadeOut();
        });
    },

    // Utility Functions
    utils: {
        formatDate: function (date) {
            return new Date(date).toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            });
        },

        formatTime: function (time) {
            return new Date('2000-01-01T' + time).toLocaleTimeString('en-US', {
                hour: '2-digit',
                minute: '2-digit',
                hour12: true
            });
        },

        copyToClipboard: function (text) {
            navigator.clipboard.writeText(text).then(function () {
                SriSaiApp.showNotification('Copied to clipboard!', 'success');
            });
        },

        downloadTable: function (tableId, filename = 'export.csv') {
            const table = document.getElementById(tableId);
            const rows = Array.from(table.querySelectorAll('tr'));

            const csvContent = rows.map(row => {
                const cells = Array.from(row.querySelectorAll('th, td'));
                return cells.map(cell => `"${cell.textContent.replace(/"/g, '""')}"`).join(',');
            }).join('\n');

            const blob = new Blob([csvContent], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            a.click();
            window.URL.revokeObjectURL(url);
        }
    }
};

// Initialize when document is ready
$(document).ready(function () {
    SriSaiApp.init();
});

// Additional jQuery plugins and extensions
$.extend({
    // Custom notification function
    notify: function (message, type = 'info') {
        SriSaiApp.showNotification(message, type);
    }
});

// Custom CSS animations for enhanced UX
const animationStyles = `
<style>
.animate-on-scroll {
    opacity: 0;
    transform: translateY(30px);
    transition: all 0.6s ease;
}

.animate-on-scroll.animate-in {
    opacity: 1;
    transform: translateY(0);
}

.search-highlight {
    background-color: rgba(255, 193, 7, 0.2) !important;
    transition: background-color 0.3s ease;
}

.loading-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(255, 255, 255, 0.9);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
    display: none;
}

.pulse {
    animation: pulse 2s infinite;
}

@keyframes pulse {
    0% {
        transform: scale(1);
    }
    50% {
        transform: scale(1.05);
    }
    100% {
        transform: scale(1);
    }
}

.slide-in-right {
    animation: slideInRight 0.5s ease;
}

@keyframes slideInRight {
    from {
        transform: translateX(100%);
        opacity: 0;
    }
    to {
        transform: translateX(0);
        opacity: 1;
    }
}

.fade-in {
    animation: fadeIn 0.5s ease;
}

@keyframes fadeIn {
    from {
        opacity: 0;
    }
    to {
        opacity: 1;
    }
}

/* Toast notification container */
#notificationContainer {
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 1050;
}

#notificationContainer .toast {
    margin-bottom: 10px;
}
</style>
`;

// Inject animation styles
document.head.insertAdjacentHTML('beforeend', animationStyles);

// Add notification container to body if it doesn't exist
if (!document.getElementById('notificationContainer')) {
    document.body.insertAdjacentHTML('beforeend', '<div id="notificationContainer"></div>');
}

// Keyboard shortcuts
document.addEventListener('keydown', function (e) {
    // Ctrl/Cmd + K for search
    if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        e.preventDefault();
        const searchInput = document.getElementById('searchInput');
        if (searchInput) {
            searchInput.focus();
        }
    }

    // Escape to close modals
    if (e.key === 'Escape') {
        $('.modal.show').modal('hide');
    }
});

// Print functionality
window.print = function () {
    window.print();
};

// Export functionality for tables
function exportTable(tableId, filename = 'export.csv') {
    SriSaiApp.utils.downloadTable(tableId, filename);
}

// Global error handler for AJAX requests
$(document).ajaxError(function (event, xhr, settings, thrownError) {
    console.error('AJAX Error:', thrownError);
    SriSaiApp.showNotification('An error occurred. Please try again.', 'danger');
});

// Service Worker registration for offline functionality (optional)
if ('serviceWorker' in navigator) {
    window.addEventListener('load', function () {
        // Uncomment to enable service worker
        // navigator.serviceWorker.register('/sw.js');
    });
}