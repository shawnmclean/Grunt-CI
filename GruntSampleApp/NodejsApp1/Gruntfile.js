module.exports = function(grunt) {
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
   
     less: {
        compile: {
            files: {
                'public/stylesheets/samplethiruuuue1wtest.css' : 'less/Sample.less' 
                }
            }
        },

     jade: {
        compile: {
            files: {
                'views/GruntIndexw.html': 'views/Gruntfileconverstiontask.jade'
                }
            }
        }
  });
   
  grunt.loadNpmTasks('grunt-contrib-less');
  grunt.loadNpmTasks('grunt-contrib-jade');
  console.log( grunt.option( "option1" ) );
  grunt.registerTask('default', ['less','jade'])
  //grunt.registerTask('myTask', '', function(){
   //var target = grunt.option('target');
//});
};