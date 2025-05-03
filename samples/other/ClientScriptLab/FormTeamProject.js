function LearnLab_handleTeamProjectOnLoad(executionContext) {
    // registers an onChange event handler and calls a common function to show/hide the section
    // You need to handle on change in case a project start date input changes the hide/show requirement.
    var formContext = executionContext.getFormContext();
    formContext.getAttribute('sample_projectstart').addOnChange(LearnLab_handleProjectStatusOnChange);
    LearnLab_hideOrShowStatusSection(formContext);
}
function LearnLab_handleProjectStatusOnChange(executionContext) {
    // gets the formContext and then calls the common function to hide/show
    var formContext = executionContext.getFormContext();
    LearnLab_hideOrShowStatusSection(formContext);
}
function LearnLab_hideOrShowStatusSection(formContext) {    
    var tabGeneral = formContext.ui.tabs.get('tab_general');
    var sectionStatus = tabGeneral.sections.get('section_status');
    var startDate = formContext.getAttribute('sample_projectstart').getValue();
    var CurrentDate = new Date();
    if (startDate == null || startDate > CurrentDate) {
      sectionStatus.setVisible(false);
    } else {
      sectionStatus.setVisible(true);
    }
}